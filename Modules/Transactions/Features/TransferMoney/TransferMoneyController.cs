using FluentMigrator;
using Microsoft.AspNetCore.Mvc;
using Transactions.DTOs;

namespace Transactions.Features.TransferMoney
{
    [Route("api/transactions")]
    [ApiController]
    [Tags("Transactions")]
    public class TransferMoneyController : ControllerBase
    {
        private readonly ITransferMoneyHandler _context;

        public TransferMoneyController(ITransferMoneyHandler context)
        {
            _context = context;
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] CreateTransferRequestDto request)
        {
            var result = await _context.HandleAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
