namespace Bank_Management_System.DTO
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<AccountDto> Accounts { get; set; }
    }

    public class AccountDto
    {
        public Guid AccountId { get; set; }
        public string AccountType { get; set; }
        public decimal? Balance { get; set; }
    }
}
