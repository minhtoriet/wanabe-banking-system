namespace Transactions.DTOs
{
    public record TransferResultDto(bool IsSuccess, string Message, Guid PaymentId, string Status);

}
