using System.Text.Json.Serialization;

namespace BankMS_API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        [JsonIgnore]
        public ICollection<Account>? Accounts { get; set; }
    }
}
