using Bank_Management_System.DTO;

namespace Bank_Management_System.Interfaces
{
    public interface IAccountsService
    {
        Task<AccountsDto> CreateAccountAsync(Guid userId, string accountType, decimal initialBalance);
        Task<object> GetAccountsByRoleAsync(string role, Guid userId, int pageNumber, int pageSize);
    }
}
