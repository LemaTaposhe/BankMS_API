using BankMS_API.Data;
using BankMS_API.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
     //[Authorize(Policy = "AdminOnly")]
    public class AccountsController : ControllerBase
    {
        private readonly BankDbContext _context;

        public AccountsController(BankDbContext context)
        {
            _context = context;
        }

        // Get all accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Transactions)
                .ToListAsync();
        }

        // Get a specific account by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null) return NotFound();

            return account;
        }

        // Create a new account (Apply for account)
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForAccount(Account account)
        {
            account.Balance = 0; // Default to zero balance when applying
            account.AccountType ??= "Savings"; // Default to 'Savings' if no type is provided
            account.Currency ??= "USD"; // Default to USD if no currency is provided

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccount), new { id = account.AccountId }, account);
        }

        // Update account information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, Account account)
        {
            if (id != account.AccountId) return BadRequest();

            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Close (Delete) an account
        [HttpDelete("{id}")]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            // Check if account balance is zero before closing
            if (account.Balance > 0)
            {
                return BadRequest("Account must have a zero balance to close.");
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Deposit money into an account
        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] decimal depositAmount)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            if (depositAmount <= 0) return BadRequest("Deposit amount must be greater than zero.");

            account.Balance += depositAmount;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Deposit successful.", NewBalance = account.Balance });
        }

        // Withdraw money from an account
        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(int id, [FromBody] decimal withdrawalAmount)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            if (withdrawalAmount <= 0) return BadRequest("Withdrawal amount must be greater than zero.");

            if (withdrawalAmount > account.Balance) return BadRequest("Insufficient funds.");

            account.Balance -= withdrawalAmount;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Withdrawal successful.", NewBalance = account.Balance });
        }

        // Transfer money between accounts
        [HttpPost("{id}/transfer/{toAccountId}")]
        public async Task<IActionResult> Transfer(int id, int toAccountId, [FromBody] decimal transferAmount)
        {
            var fromAccount = await _context.Accounts.FindAsync(id);
            var toAccount = await _context.Accounts.FindAsync(toAccountId);

            if (fromAccount == null || toAccount == null) return NotFound();

            if (transferAmount <= 0) return BadRequest("Transfer amount must be greater than zero.");
            if (transferAmount > fromAccount.Balance) return BadRequest("Insufficient funds.");

            fromAccount.Balance -= transferAmount;
            toAccount.Balance += transferAmount;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Transfer successful.", FromAccountNewBalance = fromAccount.Balance, ToAccountNewBalance = toAccount.Balance });
        }

        // Calculate account penalty for overdue payments (if applicable)
        [HttpGet("{id}/penalty")]
        public async Task<IActionResult> CalculatePenalty(int id, [FromQuery] int overdueDays)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            // Assuming penalty calculation is based on account type or any specific criteria.
            var penaltyRate = 0.01m; // Example: 1% penalty per overdue day
            var penalty = account.Balance * penaltyRate * overdueDays;

            return Ok(new { Penalty = penalty });
        }
    }
}
