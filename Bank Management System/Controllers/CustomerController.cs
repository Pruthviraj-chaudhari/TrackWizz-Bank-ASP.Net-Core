using Bank_Management_System.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{


    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;   
        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;   
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetAllCustomersAsync(pageNumber, pageSize);

                return Ok(new
                {
                    data = result.Items,
                    totalRecords = result.TotalRecords,
                    pageNumber = result.PageNumber,
                    pageSize = result.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch customers.");
                return StatusCode(500, new { message = "Internal Server Error." });
            }
        }
    }
}
