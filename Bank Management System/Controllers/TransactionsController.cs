using System.Security.Claims;
using Bank_Management_System.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("get-transactions")]
        [Authorize]  
        public async Task<IActionResult> GetTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;  
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { error = "Invalid or missing user ID in token." });
            }

            var result = await _transactionService.GetTransactionsAsync(userId, userRole, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
