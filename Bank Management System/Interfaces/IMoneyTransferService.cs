namespace Bank_Management_System.Interfaces
{
    public interface IMoneyTransferService
    {
        Task<bool> TransferMoneyAsync(string senderAccountNumber, string recipientAccountNumber, decimal amount);
    }
}
