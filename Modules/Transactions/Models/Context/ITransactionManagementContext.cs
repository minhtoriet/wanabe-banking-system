using Microsoft.EntityFrameworkCore;

namespace Transactions.Models.Context
{
    internal interface ITransactionManagementContext
    {
        DbSet<LedgerEntry> LedgerEntries { get; set; }
        DbSet<PaymentOrder> PaymentOrders { get; set; }
        //Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
