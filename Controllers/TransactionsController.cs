using BankMS_API.Data;
using BankMS_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class TransactionsController : ControllerBase
    {
        private readonly BankDbContext _context;

        public TransactionsController(BankDbContext context)
        {
            _context = context;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.AccountId);
            if (account == null) return NotFound();

            account.Balance += transaction.Amount;
            transaction.TransactionType = "Deposit";
            transaction.TransactionDate = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.AccountId);
            if (account == null) return NotFound();

            if (account.Balance < transaction.Amount) return BadRequest("Insufficient balance.");

            account.Balance -= transaction.Amount;
            transaction.TransactionType = "Withdrawal";
            transaction.TransactionDate = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(Transaction transaction)
        {
            var fromAccount = await _context.Accounts.FindAsync(transaction.AccountId);
            if (fromAccount == null) return NotFound();

            if (fromAccount.Balance < transaction.Amount) return BadRequest("Insufficient balance.");

            fromAccount.Balance -= transaction.Amount;

            var toAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == transaction.ExternalBankDetails);
            if (toAccount != null) toAccount.Balance += transaction.Amount;

            transaction.TransactionType = "Transfer";
            transaction.TransactionDate = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }
    }
}
