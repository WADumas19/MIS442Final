using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
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
            var products = dbContext.Products.ToList();
            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count > 0, "Expected to retrieve at least one product");
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            var product = dbContext.Products.Find(1);
            Assert.IsNotNull(product, "Expected to retrieve product with primary key 1");
            Assert.AreEqual(1, product.ProductCode, "ProductCode mismatch");
        }

        [Test]
        public void GetUsingWhere()
        {
            var products = dbContext.Products.Where(p => p.UnitPrice == 56.50m).ToList();
            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count > 0, "Expected to retrieve at least one product with UnitPrice 56.50");

            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            var products = dbContext.Products.Select(
                p => new { p.ProductCode, p.UnitPrice, p.OnHandQuantity, Value = p.UnitPrice * p.OnHandQuantity })
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.AreEqual(16, products.Count, "Expected 16 products in the list");

            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void DeleteTest()
        {
            var product = new Product { ProductCode = "TEST", Description = "Test Product", UnitPrice = 10.00m, OnHandQuantity = 5 };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            dbContext.Products.Remove(product);
            dbContext.SaveChanges();

            var deletedProduct = dbContext.Products.Find(product.ProductCode);
            Assert.IsNull(deletedProduct, "Expected product to be deleted");
        }

        [Test]
        public void CreateTest()
        {
            var product = new Product { ProductCode = "NEW1", Description = "New Product", UnitPrice = 20.00m, OnHandQuantity = 10 };
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            var savedProduct = dbContext.Products.Find("NEW1");
            Assert.IsNotNull(savedProduct, "Expected new product to be saved");
            Assert.AreEqual("New Product", savedProduct.Description);
        }

        [Test]
        public void UpdateTest()
        {
            var product = dbContext.Products.FirstOrDefault();
            Assert.IsNotNull(product, "Expected at least one product in the database");

            product.Description = "Updated Description";
            product.UnitPrice = 99.99m;
            dbContext.SaveChanges();

            var updatedProduct = dbContext.Products.Find(product.ProductCode);
            Assert.IsNotNull(updatedProduct);
            Assert.AreEqual("Updated Description", updatedProduct.Description);
            Assert.AreEqual(99.99m, updatedProduct.UnitPrice);
        }

    }

    internal class Product : Products
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int OnHandQuantity { get; set; }
    }
}