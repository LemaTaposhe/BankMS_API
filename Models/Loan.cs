namespace BankMS_API.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int? TermMonths { get; set; }
        public DateTime LoanDate { get; set; }
        public bool IsApproved { get; set; }
        public decimal PenaltyRate { get; set; } // Penalty for overdue payments
        public decimal? RemainingBalance { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
