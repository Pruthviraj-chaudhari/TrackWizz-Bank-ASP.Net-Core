namespace Bank_Management_System.DTO
{
    public class AccountsDto
    {
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
