using BankMS_API.Data;
using BankMS_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly BankDbContext _context;

        public LoansController(BankDbContext context)
        {
            _context = context;
        }

        // Get all loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
            return await _context.Loans
                .Include(l => l.User)
                .ToListAsync();
        }

        // Get a specific loan by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan == null) return NotFound();

            return loan;
        }

        // Apply for a loan
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForLoan(Loan loan)
        {
            loan.LoanDate = DateTime.UtcNow;
            loan.IsApproved = false; // Default to not approved
            loan.RemainingBalance = loan.PrincipalAmount;

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoan), new { id = loan.LoanId }, loan);
        }

        // Approve or reject a loan
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveLoan(int id, [FromQuery] bool isApproved)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            loan.IsApproved = isApproved;

            await _context.SaveChangesAsync();

            return Ok(new { Message = isApproved ? "Loan approved" : "Loan rejected" });
        }

        // Repay a loan
        [HttpPost("{id}/repay")]
        public async Task<IActionResult> RepayLoan(int id, [FromBody] decimal repaymentAmount)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            if (!loan.IsApproved) return BadRequest("Loan is not approved.");

            if (repaymentAmount <= 0) return BadRequest("Repayment amount must be greater than zero.");

            if (repaymentAmount > loan.RemainingBalance)
                return BadRequest("Repayment amount exceeds remaining balance.");

            loan.RemainingBalance -= repaymentAmount;

            if (loan.RemainingBalance == 0)
            {
                return Ok(new { Message = "Loan fully repaid!" });
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Repayment successful.", RemainingBalance = loan.RemainingBalance });
        }

        // Calculate penalty for overdue payments
        [HttpGet("{id}/penalty")]
        public async Task<IActionResult> CalculatePenalty(int id, [FromQuery] int overdueDays)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            if (!loan.IsApproved) return BadRequest("Loan is not approved.");

            if (overdueDays <= 0) return BadRequest("Overdue days must be greater than zero.");

            var penalty = loan.RemainingBalance * loan.PenaltyRate * overdueDays / 365;

            return Ok(new { Penalty = penalty });
        }

        // Delete a loan
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
