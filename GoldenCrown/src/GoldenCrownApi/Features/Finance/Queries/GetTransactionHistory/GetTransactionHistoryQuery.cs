using GoldenCrown.Domain.Common;
using GoldenCrownApi.Dtos.Account;
using MediatR;

namespace GoldenCrownApi.Features.Finance.Queries.GetTransactionHistory
{
    public record GetTransactionHistoryQuery
    (
        int UserId,
        DateTime? From,
        DateTime? To,
        int CurrencyId,
        int Limit,
        int Offset
    ) : IRequest<Result<List<TransactionInfo>>>;
}
