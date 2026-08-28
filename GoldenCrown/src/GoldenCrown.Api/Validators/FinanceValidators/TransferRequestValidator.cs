using FluentValidation;
using GoldenCrown.Api.Dtos.AccountDtos;

namespace GoldenCrown.Api.Validators.FinanceValidators
{
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(x => x.ReceiverLogin)
                .NotEmpty()
                .WithMessage("Логин получателя обязателен для заполнения.");

            RuleFor(x => x.FromCurrencyId)
                .GreaterThan(0)
                .WithMessage("Валюта отправителя обязательна для заполнения.");
            RuleFor(x => x.ToCurrencyId)
                .GreaterThan(0)
                .WithMessage("Валюта получателя обязательна для заполнения.");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0.01m)
                .WithMessage("Сумма должна быть больше 0.");
        }
    }
}
