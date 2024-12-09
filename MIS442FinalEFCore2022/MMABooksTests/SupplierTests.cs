using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class SupplierTests
    {
        MMABooksContext dbContext;
        Supplier? supplier;
        List<Supplier>? suppliers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            suppliers = dbContext.Suppliers.ToList();
            Assert.IsNotNull(suppliers);
            Assert.IsTrue(suppliers.Count > 0, "Expected to retrieve at least one supplier");

            PrintAll(suppliers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            supplier = dbContext.Suppliers.Find(1);
            Assert.IsNotNull(supplier, "Expected to retrieve supplier with primary key 1");
            Assert.AreEqual(1, supplier.SupplierId, "SupplierId mismatch");
            Console.WriteLine(supplier);
        }

        [Test]
        public void GetUsingWhereTest()
        {
            suppliers = dbContext.Suppliers.Where(s => s.City == "Portland").ToList();
            Assert.IsNotNull(suppliers);
            Assert.IsTrue(suppliers.Count > 0, "Expected to retrieve at least one supplier in Portland");

            PrintAll(suppliers);
        }

        [Test]
        public void CreateTest()
        {
            var newSupplier = new Supplier
            {
                Name = "New Supplier",
                ContactName = "John Doe",
                Phone = "123-456-7890",
                Email = "new.supplier@example.com",
                Address = "123 Main St",
                City = "Los Angeles",
                State = "CA",
                Zip = "90001",
                Country = "USA",
                Notes = "Test supplier notes"
            };
            dbContext.Suppliers.Add(newSupplier);
            dbContext.SaveChanges();

            var savedSupplier = dbContext.Suppliers.Find(newSupplier.SupplierId);
            Assert.IsNotNull(savedSupplier, "Expected new supplier to be saved");
            Assert.AreEqual("New Supplier", savedSupplier.Name);
        }

        [Test]
        public void UpdateTest()
        {
            supplier = dbContext.Suppliers.FirstOrDefault();
            Assert.IsNotNull(supplier, "Expected at least one supplier in the database");

            supplier.Name = "Updated Supplier";
            supplier.City = "Updated City";
            dbContext.SaveChanges();

            var updatedSupplier = dbContext.Suppliers.Find(supplier.SupplierId);
            Assert.IsNotNull(updatedSupplier);
            Assert.AreEqual("Updated Supplier", updatedSupplier.Name);
            Assert.AreEqual("Updated City", updatedSupplier.City);
        }

        [Test]
        public void DeleteTest()
        {
            var supplierToDelete = new Supplier
            {
                Name = "Test Supplier",
                City = "Test City",
                State = "CA",
                Country = "USA"
            };
            dbContext.Suppliers.Add(supplierToDelete);
            dbContext.SaveChanges();

            dbContext.Suppliers.Remove(supplierToDelete);
            dbContext.SaveChanges();

            var deletedSupplier = dbContext.Suppliers.Find(supplierToDelete.SupplierId);
            Assert.IsNull(deletedSupplier, "Expected supplier to be deleted");
        }

        public void PrintAll(List<Supplier> suppliers)
        {
            foreach (Supplier s in suppliers)
            {
                Console.WriteLine(s);
            }
        }
    }

    internal class Supplier
    {
        internal double SupplierId;
    }
}
