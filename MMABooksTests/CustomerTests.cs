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
            customers = dbContext.Customers.OrderBy(c => c.Name).ToList();
            Assert.AreEqual(696, customers.Count);
            Assert.AreEqual("Abeyatunge, Derek", customers[0].Name);
            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(1);
            Assert.IsNotNull(c);
            Assert.AreEqual("Molunguri, A", c.Name);
            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the customers who live in OR
            customers = dbContext.Customers.Where(c => c.StateCode == "OR")
                                        .OrderBy(c => c.Name)
                                        .ToList();
            Assert.AreEqual(5, customers.Count);
            PrintAll(customers);
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            // get the customer whose id is 20 and all of the invoices for that customer
            c = dbContext.Customers.Include("Invoices")
                                 .Where(c => c.CustomerId == 20)
                                 .SingleOrDefault();
            Assert.IsNotNull(c);
            Assert.IsTrue(c.Invoices.Count > 0);
            Console.WriteLine(c);
            foreach (var invoice in c.Invoices)
            {
                Console.WriteLine($"\tInvoice ID: {invoice.InvoiceId}, Total: {invoice.InvoiceTotal:C}");
            }
        }

        [Test]
        public void GetWithJoinTest()
        {
            // get a list of objects that include the customer id, name, statecode and statename
            var customers = dbContext.Customers.Join(
               dbContext.States,
               c => c.StateCode,
               s => s.StateCode,
               (c, s) => new { c.CustomerId, c.Name, c.StateCode, s.StateName }).OrderBy(r => r.StateName).ToList();
            Assert.AreEqual(691, customers.Count);
            // I wouldn't normally print here but this lets you see what each object looks like
            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }

        [Test]
        public void DeleteTest()
        {
            c = dbContext.Customers.Find(1);
            dbContext.Customers.Remove(c);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Customers.Find(1));
        }

        [Test]
        public void CreateTest()
        {
            Customer newCustomer = new Customer
            {
                Name = "Test Customer 2",
                Address = "125 Test St",
                City = "Madison",
                StateCode = "WI",
                ZipCode = "53701"
            };
            dbContext.Customers.Add(newCustomer);
            dbContext.SaveChanges();

            // Get the newly created customer
            c = dbContext.Customers.Find(newCustomer.CustomerId);
            Assert.IsNotNull(c);
            Assert.AreEqual("Test Customer 2", c.Name);
            Console.WriteLine(c);
        }

        [Test]
        public void UpdateTest()
        {
            c = dbContext.Customers.Find(1);
            c.Name = "Updated Name";
            dbContext.SaveChanges();

            // Get the updated customer
            Customer updatedCustomer = dbContext.Customers.Find(1);
            Assert.AreEqual("Updated Name", updatedCustomer.Name);
            Console.WriteLine(updatedCustomer);
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