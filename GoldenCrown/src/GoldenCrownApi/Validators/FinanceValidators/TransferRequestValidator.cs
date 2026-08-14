using FluentValidation;
using GoldenCrownApi.Dtos.Account;

namespace GoldenCrownApi.Validators.FinanceValidators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.ReceiverLogin)
                .NotEmpty()
                .WithMessage("Логин получателя обязателен для заполнения.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0)
                .WithMessage("Валюта обязательна для заполнения.");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0.01m)
                .WithMessage("Сумма должна быть больше 0.");
        }
    }
}
