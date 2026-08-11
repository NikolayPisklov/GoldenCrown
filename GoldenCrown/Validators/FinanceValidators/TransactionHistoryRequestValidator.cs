using FluentValidation;
using GoldenCrown.Dtos.Account;

namespace GoldenCrown.Validators.FinanceValidators
{
    public class TransactionHistoryRequestValidator : AbstractValidator<TransactionHistoryRequest>
    {
        public TransactionHistoryRequestValidator()
        {
            RuleFor(x => x.Limit)
                .GreaterThan(0)
                .WithMessage("Лимит должен быть больше 0.");

            RuleFor(x => x.Offset)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Смещение не может быть отрицательным.");

            RuleFor(x => x.To)
                .GreaterThanOrEqualTo(x => x.From!.Value)
                .When(x => x.From.HasValue && x.To.HasValue)
                .WithMessage("Дата окончания должна быть не раньше даты начала.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0)
                .When(x => x.CurrencyId.HasValue)
                .WithMessage("Некорректная валюта.");
        }
    }
}
