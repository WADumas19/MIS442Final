using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            customers = dbContext.Customers.ToList();
            Assert.IsNotNull(customers);
            Assert.IsTrue(customers.Count > 0, "Expected to retrieve at least one customer");

            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(1);
            Assert.IsNotNull(c, "Expected to retrieve customer with primary key 1");
            Assert.AreEqual(1, c.CustomerId, "CustomerId mismatch");
            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            customers = dbContext.Customers.Where(c => c.StateCode == "OR").ToList();
            Assert.IsNotNull(customers);
            Assert.IsTrue(customers.Count > 0, "Expected to retrieve at least one customer in Oregon");

            PrintAll(customers);
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            c = dbContext.Customers.Include(c => c.Invoices).FirstOrDefault(c => c.CustomerId == 20);
            Assert.IsNotNull(c, "Expected to retrieve customer with ID 20");
            Assert.IsNotNull(c.Invoices, "Expected customer to have invoices");

            Console.WriteLine($"Customer: {c.Name}");
            foreach (var invoice in c.Invoices)
            {
                Console.WriteLine(invoice);
            }
        }

        [Test]
        public void GetWithJoinTest()
        {
            var customersWithStateInfo = dbContext.Customers.Join(
                dbContext.States,
                c => c.StateCode,
                s => s.StateCode,
                (c, s) => new { c.CustomerId, c.Name, c.StateCode, s.StateName })
                .OrderBy(r => r.StateName)
                .ToList();

            Assert.AreEqual(696, customersWithStateInfo.Count, "Expected 696 customers in the list");

            foreach (var customer in customersWithStateInfo)
            {
                Console.WriteLine(customer);
            }
        }

        [Test]
        public void DeleteTest()
        {
            var customer = new Customer { Name = "Test Customer", StateCode = "OR", City = "Portland" };
            dbContext.Customers.Add(customer);
            dbContext.SaveChanges();

            dbContext.Customers.Remove(customer);
            dbContext.SaveChanges();

            var deletedCustomer = dbContext.Customers.Find(customer.CustomerId);
            Assert.IsNull(deletedCustomer, "Expected customer to be deleted");
        }

        [Test]
        public void CreateTest()
        {
            var newCustomer = new Customer { Name = "New Customer", StateCode = "CA", City = "Los Angeles" };
            dbContext.Customers.Add(newCustomer);
            dbContext.SaveChanges();

            var savedCustomer = dbContext.Customers.Find(newCustomer.CustomerId);
            Assert.IsNotNull(savedCustomer, "Expected new customer to be saved");
            Assert.AreEqual("New Customer", savedCustomer.Name);
        }

        [Test]
        public void UpdateTest()
        {
            c = dbContext.Customers.FirstOrDefault();
            Assert.IsNotNull(c, "Expected at least one customer in the database");

            c.Name = "Updated Name";
            c.City = "Updated City";
            dbContext.SaveChanges();

            var updatedCustomer = dbContext.Customers.Find(c.CustomerId);
            Assert.IsNotNull(updatedCustomer);
            Assert.AreEqual("Updated Name", updatedCustomer.Name);
            Assert.AreEqual("Updated City", updatedCustomer.City);
        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }

    }
}