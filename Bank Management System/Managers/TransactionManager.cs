using Bank_Management_System.Interfaces;
using Bank_Management_System.models;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_System.Managers
{
    public class TransactionManager : ITransactionService
    {
        private readonly BankDbContext _context;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TransactionManager));

        public TransactionManager(BankDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetTransactionsAsync(Guid userId, string userRole, int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                _logger.Error("Invalid pagination parameters.");
                throw new ArgumentException("Invalid pagination parameters.");
            }

            IQueryable<Transaction> query = _context.Transactions
                .Include(t => t.Account)
                .OrderByDescending(t => t.CreatedAt);

      
            if (userRole != "Admin")
            {
                query = query.Where(t => t.Account.UserId == userId);
            }

            var totalRecords = await query.CountAsync();
            var transactions = await query.Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .Select(t => new
                                          {
                                              t.TransactionId,
                                              t.AccountId,
                                              t.Type,
                                              t.Amount,
                                              t.RecipientAccountNumber,
                                              t.Status,
                                              t.CreatedAt,
                                              AccountNumber = t.Account.AccountNumber
                                          })
                                          .ToListAsync();

            return new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Transactions = transactions
            };
        }
    }
}
