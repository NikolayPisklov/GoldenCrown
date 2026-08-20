using GoldenCrown.Application.Dtos;
using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Finance.Queries.GetTransactionHistory
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
