using Bank_Management_System.DTO;
using Bank_Management_System.Interfaces;
using Bank_Management_System.models;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_System.Managers
{
    public class CustomerManager: ICustomerService
    {

        private readonly BankDbContext _dbContext;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CustomerManager));
        public CustomerManager(BankDbContext dbContext) {
            _dbContext = dbContext; 
        }

        public async Task<PagedResult<CustomerDto>> GetAllCustomersAsync(int pageNumber, int pageSize)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    _logger.Error("Page number and size must be greater than 0.");
                    throw new ArgumentException("Page number and size must be greater than 0.");
                }
                    

                var query = _dbContext.Users
                    .Where(c => c.Role == "Customer")
                    .Include(c => c.Accounts)
                    .AsQueryable();

                var totalRecords = await query.CountAsync();

                var customers = await query
                    .OrderBy(c => c.FullName) 
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CustomerDto
                    {
                        CustomerId = c.UserId,
                        FullName = c.FullName,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt,
                        Accounts = c.Accounts.Select(a => new AccountDto
                        {
                            AccountId = a.AccountId,
                            AccountType = a.AccountType,
                            Balance = a.Balance
                        }).ToList()
                    })
                    .ToListAsync();

                return new PagedResult<CustomerDto>(customers, totalRecords, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving customers.");
                throw new ApplicationException("An error occurred while fetching customers.");
            }
        }

    }
}
