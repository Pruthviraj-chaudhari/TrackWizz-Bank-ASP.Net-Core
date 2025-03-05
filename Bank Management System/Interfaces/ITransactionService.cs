namespace Bank_Management_System.Interfaces
{
    public interface ITransactionService
    {
        Task<object> GetTransactionsAsync(Guid userId, string userRole, int pageNumber, int pageSize);
    }
}
