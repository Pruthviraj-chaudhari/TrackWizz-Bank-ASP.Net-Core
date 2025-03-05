using Bank_Management_System.Interfaces;
using Bank_Management_System.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/admin/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MoneyTransferController : ControllerBase
    {
        private readonly IMoneyTransferService _moneyTransferService;

        public MoneyTransferController(IMoneyTransferService moneyTransferService)
        {
            _moneyTransferService = moneyTransferService;
        }

        [HttpPost("money-transfer")]
        public async Task<IActionResult> TransferMoney([FromBody] MoneyTransferRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.SenderAccountNumber) ||
                    string.IsNullOrWhiteSpace(request.RecipientAccountNumber) || request.Amount <= 0)
                {
                    return BadRequest("Invalid request data.");
                }

                var success = await _moneyTransferService.TransferMoneyAsync(request.SenderAccountNumber, request.RecipientAccountNumber, request.Amount);

                if (success)
                    return Ok(new { Message = "Transfer successful." });

                return BadRequest("Transfer failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DTO for transfer request
        public class MoneyTransferRequest
        {
            public string SenderAccountNumber { get; set; }
            public string RecipientAccountNumber { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
