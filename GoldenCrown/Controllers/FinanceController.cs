using GoldenCrown.Dtos.Account;
using GoldenCrown.Services.FinanceServices;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeService;
        
        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;   
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance(string token)
        {
            var result = await _financeService.GetBalanceAsync(token);
            if (result)
            {
                return Ok(new BalanceResponse { Balance = result.Value });
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var result = await _financeService.DepositAsync(request.Token, request.Amount);
            if (result)
            {
                return Ok(new BalanceResponse { Balance = result.Value});
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequest request)
        {
            var result = await _financeService.TransferAsync(request.Token, request.ReceiverLogin, request.Amount);
            if (result)
            {
                return Ok(new BalanceResponse { Balance = result.Value });
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
    }
}
