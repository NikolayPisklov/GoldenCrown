using FluentValidation;
using GoldenCrown.Dtos.Account;

namespace GoldenCrown.Validators.FinanceValidators
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
