using Bank_Management_System.DTO;

namespace Bank_Management_System.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerDto>> GetAllCustomersAsync(int pageNumber, int pageSize);


    }
}
