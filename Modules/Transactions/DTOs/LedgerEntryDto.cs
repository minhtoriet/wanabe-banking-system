namespace Transactions.DTOs
{
    public record LedgerEntryDto(Guid EntryId, Guid PaymentId, string TransactionType, double Amount, DateTime CreatedAt);
}
