using Transactions.DTOs;

namespace Transactions.Features.TransferMoney
{
    public interface ITransferMoneyHandler
    {
        Task<TransferResultDto> HandleAsync(CreateTransferRequestDto request);
    }
}
