using Bank_Management_System.Interfaces;
using Bank_Management_System.models;
using log4net;
using Microsoft.EntityFrameworkCore;

namespace Bank_Management_System.Managers
{
    public class MoneyTransferManager : IMoneyTransferService
    {
        private readonly BankDbContext _dbContext;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MoneyTransferManager));

        public MoneyTransferManager(BankDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<bool> TransferMoneyAsync(string senderAccountNumber, string recipientAccountNumber, decimal amount) {
            
            if (amount <= 0) 
            {
                _logger.Error("Transfer amount must be greater than zero.");
                throw new ArgumentException("Transfer amount must be greater than zero.");
            }
                

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var senderAcc = await _dbContext.Accounts.
                    FirstOrDefaultAsync(a => a.AccountNumber == senderAccountNumber && a.Status == "Active");

                var receiverAcc = await _dbContext.Accounts.
                   FirstOrDefaultAsync(a => a.AccountNumber == recipientAccountNumber && a.Status == "Active");

                if (senderAcc == null) {
                    _logger.Error($"Sender account not found or inactive: {senderAccountNumber}");
                    throw new InvalidOperationException("Sender account not found or inactive");
                } 
                if (receiverAcc == null) {
                    _logger.Error($"Recipient account not found or inactive: {recipientAccountNumber}");
                    throw new InvalidOperationException("Recipient  account not found or inactive");
                }
                if(senderAcc.Balance < amount)
                {
                    _logger.Error($"Insufficient balance in sender's account: {senderAccountNumber}");
                    throw new InvalidOperationException("Insufficient balance in sender's account.");
                }

                senderAcc.Balance -= amount;
                _dbContext.Accounts.Update(senderAcc);  

                receiverAcc.Balance += amount;
                _dbContext.Accounts.Update(receiverAcc);

                var senderTransaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    AccountId = senderAcc.AccountId,
                    Type = "Debit",
                    Amount = amount,
                    RecipientAccountNumber = recipientAccountNumber,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.Transactions.AddAsync(senderTransaction);

                var recipientTransaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    AccountId = receiverAcc.AccountId,
                    Type = "Credit",
                    Amount = amount,
                    RecipientAccountNumber = senderAccountNumber,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.Transactions.AddAsync(recipientTransaction);

                await _dbContext.SaveChangesAsync();    
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                _logger.Error($"Error occured while money transfer from {senderAccountNumber} to {recipientAccountNumber}");
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
