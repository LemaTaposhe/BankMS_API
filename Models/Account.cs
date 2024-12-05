
using System.Text.Json.Serialization;

namespace BankMS_API.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string? AccountType { get; set; } // Savings, Checking, Loan
        public string? Currency { get; set; } // e.g., USD, EUR, INR ,BDT
        public int UserId { get; set; }
        
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
