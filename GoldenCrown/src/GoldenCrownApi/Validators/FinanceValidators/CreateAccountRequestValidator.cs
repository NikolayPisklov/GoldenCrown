using FluentValidation;
using GoldenCrownApi.Dtos.Account;

namespace GoldenCrownApi.Validators.FinanceValidators
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator()
        {
            RuleFor(x => x.CurrencyId)
                .GreaterThan(0)
                .WithMessage("Валюта обязательна для заполнения.");
        }
    }
}
