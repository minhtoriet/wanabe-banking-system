namespace Transactions.DTOs
{
    public record PaymentOrderDto(Guid PaymentId, Guid DebtorAccountId, Guid CreditorAccountId, double Amount, string Status, DateTime CreatedAt);
}
