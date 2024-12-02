
namespace BankMS_API.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string? AccountType { get; set; } // Savings, Checking, Loan
        public string? Currency { get; set; } // e.g., USD, EUR, INR
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
