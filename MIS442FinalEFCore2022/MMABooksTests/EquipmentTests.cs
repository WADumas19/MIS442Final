using System.Collections.Generic;
using System.Linq;
using System;
using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class EquipmentTests
    {
        MMABooksContext dbContext;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            var equipmentList = dbContext.Equipments.ToList();
            Assert.IsNotNull(equipmentList);
            Assert.IsTrue(equipmentList.Count > 0, "Expected to retrieve at least one equipment record");
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            var equipment = dbContext.Equipments.Find(1);
            Assert.IsNotNull(equipment, "Expected to retrieve equipment with primary key 1");
            Assert.AreEqual(1, equipment.EquipmentId, "EquipmentId mismatch");
        }

        [Test]
        public void GetUsingWhere()
        {
            var equipmentList = dbContext.Equipments.Where(e => e.Version == 2).ToList();
            Assert.IsNotNull(equipmentList);
            Assert.IsTrue(equipmentList.Count > 0, "Expected to retrieve at least one equipment record with Version 2");

            foreach (var e in equipmentList)
            {
                Console.WriteLine(e);
            }
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            var equipmentList = dbContext.Equipments.Select(
                e => new
                {
                    e.EquipmentId,
                    e.Name,
                    BoilCapacity = e.BoilSize * (1 - (e.TrubChillerLoss / 100))
                })
                .OrderBy(e => e.EquipmentId)
                .ToList();

            Assert.IsTrue(equipmentList.Count > 0, "Expected at least one equipment record in the list");

            foreach (var e in equipmentList)
            {
                Console.WriteLine(e);
            }
        }

        [Test]
        public void DeleteTest()
        {
            var equipment = new Equipment
            {
                Name = "Test Equipment",
                Version = 1,
                BoilSize = 10.5f,
                Notes = "Temporary test equipment"
            };
            dbContext.Equipments.Add(equipment);
            dbContext.SaveChanges();

            dbContext.Equipments.Remove(equipment);
            dbContext.SaveChanges();

            var deletedEquipment = dbContext.Equipments.Find(equipment.EquipmentId);
            Assert.IsNull(deletedEquipment, "Expected equipment to be deleted");
        }

        [Test]
        public void CreateTest()
        {
            var equipment = new Equipment
            {
                Name = "New Equipment",
                Version = 1,
                BoilSize = 20.5f,
                BatchSize = 15.0f,
                Notes = "Created during test"
            };
            dbContext.Equipments.Add(equipment);
            dbContext.SaveChanges();

            var savedEquipment = dbContext.Equipments.SingleOrDefault(e => e.Name == "New Equipment");
            Assert.IsNotNull(savedEquipment, "Expected new equipment to be saved");
            Assert.AreEqual("New Equipment", savedEquipment.Name);
        }

        [Test]
        public void UpdateTest()
        {
            var equipment = dbContext.Equipments.FirstOrDefault();
            Assert.IsNotNull(equipment, "Expected at least one equipment record in the database");

            equipment.Name = "Updated Equipment";
            equipment.Version = 3;
            dbContext.SaveChanges();

            var updatedEquipment = dbContext.Equipments.Find(equipment.EquipmentId);
            Assert.IsNotNull(updatedEquipment);
            Assert.AreEqual("Updated Equipment", updatedEquipment.Name);
            Assert.AreEqual(3, updatedEquipment.Version);
        }
    }

    internal class Equipment : Equipments
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; }
        public int? Version { get; set; }
        public float? BoilSize { get; set; }
        public float? BatchSize { get; set; }
        public string Notes { get; set; }
    }

    internal class Equipments
    {
    }
}
