using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace MMABooksTests
{
    [TestFixture]
    public class AccountTests
    {
        MMABooksContext dbContext;
        Account? account;
        List<Account>? accounts;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            accounts = dbContext.Accounts.OrderBy(a => a.Name).ToList();
            Assert.IsNotNull(accounts);
            Assert.IsTrue(accounts.Count > 0, "Expected at least one account to be retrieved.");
            PrintAll(accounts);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            account = dbContext.Accounts.Find(1);
            Assert.IsNotNull(account, "Expected to retrieve an account with ID 1.");
            Console.WriteLine(account);
        }

        [Test]
        public void GetUsingWhereTest()
        {
            accounts = dbContext.Accounts.Where(a => a.Balance > 1000).OrderBy(a => a.Name).ToList();
            Assert.IsNotNull(accounts);
            Assert.IsTrue(accounts.Count > 0, "Expected to retrieve accounts with a balance greater than 1000.");
            PrintAll(accounts);
        }

        [Test]
        public void DeleteTest()
        {
            account = dbContext.Accounts.Find(1);
            Assert.IsNotNull(account, "Expected to find the account to delete.");

            dbContext.Accounts.Remove(account);
            dbContext.SaveChanges();

            Assert.IsNull(dbContext.Accounts.Find(1), "Expected the account to be deleted.");
        }

        [Test]
        public void CreateTest()
        {
            var newAccount = new Account
            {
                Name = "New Account",
                Balance = 500.00m,
                Notes = "This is a test account."
            };

            dbContext.Accounts.Add(newAccount);
            dbContext.SaveChanges();

            var savedAccount = dbContext.Accounts.Find(newAccount.AccountId);
            Assert.IsNotNull(savedAccount, "Expected the new account to be saved.");
            Assert.AreEqual("New Account", savedAccount.Name);
        }

        [Test]
        public void UpdateTest()
        {
            account = dbContext.Accounts.FirstOrDefault();
            Assert.IsNotNull(account, "Expected at least one account in the database.");

            account.Name = "Updated Account Name";
            account.Balance = 750.00m;
            dbContext.SaveChanges();

            var updatedAccount = dbContext.Accounts.Find(account.AccountId);
            Assert.IsNotNull(updatedAccount, "Expected the updated account to be retrieved.");
            Assert.AreEqual("Updated Account Name", updatedAccount.Name);
            Assert.AreEqual(750.00m, updatedAccount.Balance);
        }

        public void PrintAll(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                Console.WriteLine(account);
            }
        }
    }

    internal class Account
    {
    }
}
