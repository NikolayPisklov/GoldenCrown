using FluentValidation;
using GoldenCrownApi.Dtos.Account;

namespace GoldenCrownApi.Validators.FinanceValidators
{
    public class DepositRequestValidator : AbstractValidator<DepositRequest>
    {
        public DepositRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Сумма должна быть больше 0.");
        }
    }
}
