using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;
using MMABooksEFClasses.Models;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        
        MMABooksEFClasses.MarisModels.MMABooksContext dbContext;
        Products? p;
        List<Products>? products;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksEFClasses.MarisModels.MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            products = dbContext.Products.OrderBy(p => p.Description).ToList();
            Assert.AreEqual(16, products.Count);
            PrintAll(products);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            p = dbContext.Products.Find("A4CS");
            Assert.IsNotNull(p);
            Assert.AreEqual("A4CS", p.ProductCode);
            Console.WriteLine(p);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the products that have a unit price of 56.50
            // get a list of all of the products that have a unit price of 56.50
            products = dbContext.Products.Where(p => p.UnitPrice == 56.50m)
                                       .OrderBy(p => p.Description)
                                       .ToList();
            PrintAll(products);
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            // get a list of objects that include the productcode, unitprice, quantity and inventoryvalue
            var products = dbContext.Products.Select(
            p => new { p.ProductCode, p.UnitPrice, p.OnHandQuantity, Value = p.UnitPrice * p.OnHandQuantity }).
            OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void DeleteTest()
        {
            // First find a product with its related invoice line items
            p = dbContext.Products.Include(p => p.Invoicelineitems)
                                 .Where(p => p.ProductCode == "A4CS")
                                 .SingleOrDefault();
            Assert.IsNotNull(p);  // Verify we found it

            // Remove all related invoice line items first
            dbContext.Invoicelineitems.RemoveRange(p.Invoicelineitems);
            dbContext.SaveChanges();

            // Now we can remove the product
            dbContext.Products.Remove(p);
            dbContext.SaveChanges();

            // Try to find the deleted product - should be null
            Products? deletedProduct = dbContext.Products.Find("A4CS");
            Assert.IsNull(deletedProduct);
        }

        [Test]
        public void CreateTest()
        {
            Products newProduct = new Products
            {
                ProductCode = "TEST1",
                Description = "Test Product",
                UnitPrice = 29.99m,
                OnHandQuantity = 100
            };

            dbContext.Products.Add(newProduct);  // Add the new product
            dbContext.SaveChanges();  // Save changes to database

            // Try to find the newly created product
            Products? foundProduct = dbContext.Products.Find("TEST1");
            Assert.IsNotNull(foundProduct);  // Verify it exists
            Assert.AreEqual("Test Product", foundProduct.Description);
            Assert.AreEqual(29.99m, foundProduct.UnitPrice);
            Assert.AreEqual(100, foundProduct.OnHandQuantity);

        }

        [Test]
        public void UpdateTest()
        {
            // First find a product that we know exists
            p = dbContext.Products.Find("A4CS");
            Assert.IsNotNull(p);  // Verify we found it

            // Update the product's properties
            string originalDescription = p.Description;
            decimal originalPrice = p.UnitPrice;

            p.Description = "Updated Description";
            p.UnitPrice = 59.99m;
            dbContext.SaveChanges();  // Save changes to database

            // Retrieve the product again to verify changes
            Products? updatedProduct = dbContext.Products.Find("A4CS");
            Assert.IsNotNull(updatedProduct);
            Assert.AreEqual("Updated Description", updatedProduct.Description);
            Assert.AreEqual(59.99m, updatedProduct.UnitPrice);

        }

        public void PrintAll(List<Products> products)
        {
            foreach (Products p in products)
            {
                Console.WriteLine($"ProductCode: {p.ProductCode}, Description: {p.Description}, " +
                                 $"UnitPrice: {p.UnitPrice:C}, OnHandQuantity: {p.OnHandQuantity}");
            }
        }


    }
}