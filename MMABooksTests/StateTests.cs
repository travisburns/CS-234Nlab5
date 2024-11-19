using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class StateTests
    {
        // ignore this warning about making dbContext nullable.
        // if you add the ?, you'll get a warning wherever you use dbContext
        MMABooksContext dbContext;
        State? s;
        List<State>? states;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            states = dbContext.States.OrderBy(s => s.StateName).ToList();
            Assert.AreEqual(53, states.Count);
            Assert.AreEqual("Alabama", states[0].StateName);
            PrintAll(states);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            s = dbContext.States.Find("WI");
            Assert.IsNotNull(s);
            Assert.AreEqual("Wisconsi", s.StateName);
            Console.WriteLine(s);
        }

        [Test]
        public void GetUsingWhere()
        {
            states = dbContext.States.Where(s => s.StateName.StartsWith("A")).OrderBy(s => s.StateName).ToList();
            Assert.AreEqual(4, states.Count);
            Assert.AreEqual("Alabama", states[0].StateName);
            PrintAll(states);
        }

        [Test]
        public void GetWithCustomersTest()
        {
            s = dbContext.States.Include("Customers").Where(s => s.StateCode == "WI").SingleOrDefault();
            Assert.IsNotNull(s);
            Assert.AreEqual("Wisconsi", s.StateName);
            Assert.AreEqual(17, s.Customers.Count);
            Console.WriteLine(s);
        }

        [Test]
        public void DeleteTest()
        {
           
            s = dbContext.States.Include(s => s.Customers)
                               .Where(s => s.StateCode == "TT")
                               .SingleOrDefault();
            Assert.IsNotNull(s);  

           
            dbContext.Customers.RemoveRange(s.Customers);
            dbContext.SaveChanges();

          
            dbContext.States.Remove(s);
            dbContext.SaveChanges();

           
            Assert.IsNull(dbContext.States.Find("TT"));
        }

        [Test]
        public void CreateTest()
        {
            // Create a new state
            State newState = new State
            {
                StateCode = "TT",  // Test State
                StateName = "Test State"
            };

            dbContext.States.Add(newState);
            dbContext.SaveChanges();

            // Verify the state was created
            s = dbContext.States.Find("TT");
            Assert.IsNotNull(s);
            Assert.AreEqual("Test State", s.StateName);
            Console.WriteLine(s);
        }

        [Test]
        public void UpdateTest()
        {
            // Find an existing state
            s = dbContext.States.Find("WI");
            Assert.IsNotNull(s);

            // Store original name for comparison/display
            string originalName = s.StateName;

            // Update the state
            s.StateName = "Updated Wisconson";
            dbContext.SaveChanges();

            // Verify the update
            State updatedState = dbContext.States.Find("WI");
            Assert.AreEqual("Updated Wisconson", updatedState.StateName);

            // Print the changes
            Console.WriteLine($"Original Name: {originalName}");
            Console.WriteLine($"Updated Name: {updatedState.StateName}");
        }

        public void PrintAll(List<State> states)
        {
            foreach (State s in states)
            {
                Console.WriteLine(s);
            }
        }
    }
}