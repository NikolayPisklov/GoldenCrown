using FluentValidation;
using GoldenCrown.Api.Dtos.AccountDtos;

namespace GoldenCrown.Api.Validators.FinanceValidators
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
