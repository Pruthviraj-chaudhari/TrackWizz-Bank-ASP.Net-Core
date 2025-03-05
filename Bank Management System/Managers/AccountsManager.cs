using Bank_Management_System.DTO;
using Bank_Management_System.Interfaces;
using Bank_Management_System.models;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_System.Managers
{
    public class AccountsManager : IAccountsService
    {
        private readonly BankDbContext _context;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AccountsManager));

        public AccountsManager(BankDbContext context)
        {
            _context = context;
        }

        public async Task<AccountsDto> CreateAccountAsync(Guid userId, string accountType, decimal initialBalance)
        {
    
            var user = await _context.Users.FindAsync(userId);
            if (user == null) {
                _logger.Error($"User not found: {userId}");
                throw new Exception("User not found");
            }
          
            string accountNumber = GenerateAccountNumber();

            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                UserId = userId,
                AccountNumber = accountNumber,
                AccountType = accountType,
                Balance = initialBalance,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new AccountsDto
            {
                AccountId = account.AccountId,
                AccountNumber = account.AccountNumber,
                UserId = account.UserId.Value,
                FullName = user.FullName,
                AccountType = account.AccountType!,
                Balance = account.Balance!.Value,
                Status = account.Status!
            };
        }

        public async Task<object> GetAccountsByRoleAsync(string role, Guid userId, int pageNumber, int pageSize)
        {
            IQueryable<Account> query = _context.Accounts.Include(a => a.User);

            if (role == "Customer")
            {
                query = query.Where(a => a.UserId == userId);
            }

            int totalRecords = await query.CountAsync();

            var accounts = await query
                .OrderBy(a => a.AccountId) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AccountsDto
                {
                    AccountId = a.AccountId,
                    AccountNumber = a.AccountNumber,
                    UserId = a.UserId!.Value,
                    FullName = a.User!.FullName,
                    AccountType = a.AccountType!,
                    Balance = a.Balance!.Value,
                    Status = a.Status!,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = accounts
            };
        }


        private string GenerateAccountNumber()
        {
            return "ACC" + DateTime.UtcNow.Ticks.ToString()[^10..];
        }
    }
}
