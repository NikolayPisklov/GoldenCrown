using FluentValidation;
using GoldenCrown.Attributes;
using GoldenCrown.Dtos.Account;
using GoldenCrown.Extentions;
using GoldenCrown.Services.FinanceServices;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [GoldenCrownAuth]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeService;
        private readonly IValidator<TransferRequest> _transferValidator;
        private readonly IValidator<TransactionHistoryRequest> _historyValidator;
        private readonly IValidator<DepositRequest> _depositValidator;

        public FinanceController(IFinanceService financeService, IValidator<TransactionHistoryRequest> historyValidator, IValidator<DepositRequest> depositValidator, IValidator<TransferRequest> transferValidator)
        {
            _financeService = financeService;
            _historyValidator = historyValidator;
            _depositValidator = depositValidator;
            _transferValidator = transferValidator;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = HttpContext.GetUserId();
            var result = await _financeService.GetBalanceAsync(userId);
            if (result)
            {
                return Ok(new BalanceResponse { Balance = result.Value });
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpGet("get-history")]
        public async Task<IActionResult> GetHistory([FromQuery]TransactionHistoryRequest request)
        {
            var validationResult = _historyValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validationResult.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
            var userId = HttpContext.GetUserId();
            var result = await _financeService.GetTransactionHistoryAsync(userId, request.From, request.To, request.Limit, request.Offset);
            if (result)
            {
                return Ok(new TransactionHistoryResponse() { Transactions = result.Value!});
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            var validationResult = _depositValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validationResult.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
            var userId = HttpContext.GetUserId();
            var result = await _financeService.DepositAsync(userId, request.Amount);
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
            var validationResult = _transferValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validationResult.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
            var userId = HttpContext.GetUserId();
            var result = await _financeService.TransferAsync(userId, request.ReceiverLogin, request.Amount);
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
