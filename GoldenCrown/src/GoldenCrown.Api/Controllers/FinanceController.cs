using FluentValidation;
using GoldenCrown.Api.Dtos.AccountDtos;
using GoldenCrown.Api.Extentions;
using GoldenCrown.Api.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GoldenCrown.Application.Features.Finance.Queries.GetBalance;
using GoldenCrown.Application.Features.Finance.Commands.Deposit;
using GoldenCrown.Application.Features.Finance.Queries.GetTransactionHistory;
using GoldenCrown.Application.Dtos;
using GoldenCrown.Application.Features.Finance.Commands.CreateAccount;
using GoldenCrown.Application.Features.Finance.Commands.Transfer;

namespace GoldenCrown.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [GoldenCrownAuth]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<TransferRequest> _transferValidator;
        private readonly IValidator<TransactionHistoryRequest> _historyValidator;
        private readonly IValidator<DepositRequest> _depositValidator;
        private readonly IValidator<CreateAccountRequest> _createAccountValidator;

        public FinanceController(IMediator mediator, IValidator<TransactionHistoryRequest> historyValidator, IValidator<DepositRequest> depositValidator, IValidator<TransferRequest> transferValidator, IValidator<CreateAccountRequest> createAccountValidator)
        {
            _mediator = mediator;
            _historyValidator = historyValidator;
            _depositValidator = depositValidator;
            _transferValidator = transferValidator;
            _createAccountValidator = createAccountValidator;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance(CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            var result = await _mediator.Send(new GetBalanceQuery(userId), cancellationToken);
            if (result)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpGet("get-history")]
        public async Task<IActionResult> GetHistory([FromQuery]TransactionHistoryRequest request, CancellationToken cancellationToken)
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
            var result = await _mediator.Send(new GetTransactionHistoryQuery(userId, request.From, request.To, request.CurrencyId, request.Limit, request.Offset), cancellationToken);
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
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request, CancellationToken cancellationToken)
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
            var result = await _mediator.Send(new DepositCommand(userId, request.Amount, request.CurrencyId), cancellationToken);
            if (result)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequest request, CancellationToken cancellationToken)
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
            var result = await _mediator.Send(new TransferCommand(userId, request.ReceiverLogin, request.Amount, request.FromCurrencyId, request.ToCurrencyId), cancellationToken);
            if (result)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest request, CancellationToken cancellationToken) 
        {
            var validationResult = _createAccountValidator.Validate(request);
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
            var result = await _mediator.Send(new CreateAccountCommand(userId, request.CurrencyId), cancellationToken);
            if (result)
            {
                return Ok("Счёт создан.");
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
    }
}
