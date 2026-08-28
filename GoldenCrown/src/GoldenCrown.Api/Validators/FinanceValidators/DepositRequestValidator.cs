using FluentValidation;
using GoldenCrown.Api.Dtos.AccountDtos;

namespace GoldenCrown.Api.Validators.FinanceValidators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.CurrencyFromId)
                .GreaterThan(0)
                .WithMessage("Валюта пополнения обязательна для заполнения.");
            RuleFor(x => x.CurrencyToId)
                .GreaterThan(0)
                .WithMessage("Валюта счёта обязательна для заполнения.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Сумма должна быть больше 0.");
        }
    }
}
