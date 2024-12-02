using BankMS_API.Data;
using BankMS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace BankMS_API.Services
{
    public class InterestCalculationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public InterestCalculationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();

                // Fetch accounts eligible for interest calculation
                var accounts = await dbContext.Accounts
                    .Where(a => a.AccountType == "Savings")
                    .ToListAsync();

                foreach (var account in accounts)
                {
                    // Calculate interest (e.g., 2% per month)
                    var interest = account.Balance * 0.02m;
                    account.Balance += interest;

                    // Record the interest transaction
                    dbContext.Transactions.Add(new Transaction
                    {
                        AccountId = account.AccountId,
                        Amount = interest,
                        TransactionType = "Interest",
                        TransactionDate = DateTime.UtcNow,
                        Description = "Monthly interest credited"
                    });
                }

                await dbContext.SaveChangesAsync();

                // Wait for 30 days (or adjust based on testing)
                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
               
            }
        }
    }
}
