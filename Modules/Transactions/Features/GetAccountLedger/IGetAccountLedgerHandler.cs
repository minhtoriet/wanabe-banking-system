using Transactions.DTOs;

namespace Transactions.Features.GetAccountLedger
{
    public interface IGetAccountLedgerHandler
    {
        Task<IEnumerable<LedgerEntryDto>> HandleAsync(Guid accountId);
    }
}
