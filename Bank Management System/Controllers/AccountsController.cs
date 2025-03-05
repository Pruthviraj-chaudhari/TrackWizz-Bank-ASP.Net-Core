using System.Security.Claims;
using Bank_Management_System.Interfaces;
using Bank_Management_System.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Management_System.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountService;

        public AccountsController(IAccountsService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (request.InitialBalance < 0)
                return BadRequest("Initial balance cannot be negative");

            try
            {
                var account = await _accountService.CreateAccountAsync(request.UserId, request.AccountType, request.InitialBalance);
                return CreatedAtAction(nameof(GetAllAccounts), new { id = account.AccountId }, account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAccounts(int pageNumber = 1, int pageSize = 10)
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return Unauthorized(new { message = "Invalid user credentials" });
            }

            Guid userId = Guid.Parse(userIdClaim);

            var accounts = await _accountService.GetAccountsByRoleAsync(roleClaim, userId, pageNumber, pageSize);

            return Ok(accounts);
        }


        public class CreateAccountRequest
        {
            public Guid UserId { get; set; }
            public string AccountType { get; set; } = string.Empty;
            public decimal InitialBalance { get; set; }
        }
    }
}
