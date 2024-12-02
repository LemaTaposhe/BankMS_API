using BankMS_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankMS_API.Data
{
    public class SeedData
    {
        public static void Initialize(BankDbContext context)
        {
            context.Database.EnsureCreated(); // Ensures that the database is created if it doesn't exist

            // Check if the data is already seeded
            if (context.Users.Any())
                return; // Data has already been seeded

            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    FullName = "John Doe",
                    Email = "johndoe@example.com",
                    PasswordHash = "hashedpassword1", // For demonstration, use proper hashing in real scenarios
                    Role = "Admin"
                },
                new User
                {
                    FullName = "Jane Smith",
                    Email = "janesmith@example.com",
                    PasswordHash = "hashedpassword2",
                    Role = "Customer"
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges(); // Save Users to the database

            // Seed Accounts for Users
            var accounts = new List<Account>
            {
                new Account
                {
                    AccountNumber = "ACC1001",
                    Balance = 1000.00M,
                    AccountType = "Savings",
                    Currency = "USD",
                    UserId = users[0].UserId // Link to User
                },
                new Account
                {
                    AccountNumber = "ACC1002",
                    Balance = 500.00M,
                    AccountType = "Checking",
                    Currency = "USD",
                    UserId = users[1].UserId // Link to User
                }
            };

            context.Accounts.AddRange(accounts);
            context.SaveChanges(); // Save Accounts to the database

            // Seed Transactions for Accounts
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = 200.00M,
                    TransactionType = "Deposit",
                    Description = "Initial Deposit",
                    AccountId = accounts[0].AccountId
                },
                new Transaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = 100.00M,
                    TransactionType = "Withdrawal",
                    Description = "ATM Withdrawal",
                    AccountId = accounts[1].AccountId
                }
            };

            context.Transactions.AddRange(transactions);
            context.SaveChanges(); // Save Transactions to the database

            // Seed Loans for Users
            var loans = new List<Loan>
            {
                new Loan
                {
                    PrincipalAmount = 5000.00M,
                    InterestRate = 5.0M,
                    TermMonths = 24,
                    LoanDate = DateTime.Now,
                    IsApproved = true,
                    PenaltyRate = 2.0M,
                    RemainingBalance = 4500.00M,
                    UserId = users[0].UserId // Link to User
                },
                new Loan
                {
                    PrincipalAmount = 10000.00M,
                    InterestRate = 7.0M,
                    TermMonths = 36,
                    LoanDate = DateTime.Now,
                    IsApproved = true,
                    PenaltyRate = 3.0M,
                    RemainingBalance = 9500.00M,
                    UserId = users[1].UserId // Link to User
                }
            };

            context.Loans.AddRange(loans);
            context.SaveChanges(); // Save Loans to the database
        }
    }
}
