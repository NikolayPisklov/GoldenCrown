using GoldenCrown.Common;
using GoldenCrown.Dtos.Account;
using MediatR;

namespace GoldenCrown.Features.Finance.Queries.GetTransactionHistory
{
    public record GetTransactionHistoryQuery
    (
        int UserId,
        DateTime? From,
        DateTime? To,
        int? CurrencyId,
        int Limit,
        int Offset
    ) : IRequest<Result<List<TransactionInfo>>>;
}
