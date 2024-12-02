namespace BankMS_API.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; } // Deposit, Withdrawal, Transfer
        public string? Description { get; set; } // e.g., "Transfer to X account"
        public decimal? Fee { get; set; } // Transaction fee, if applicable
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public string? ExternalBankDetails { get; set; } // For external transfers
    }
}
