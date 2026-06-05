using FluentMigrator;
using Microsoft.AspNetCore.Mvc;

namespace Transactions.Features.GetPaymentOrders
{
    [ApiController]
    [Route("api/transactions")]
    [Tags("Transactions")]
    public class GetPaymentOrdersController : ControllerBase
    {
        private readonly IGetPaymentOrdersHandler _context;

        public GetPaymentOrdersController(IGetPaymentOrdersHandler context)
        {
            _context = context;
        }
        [HttpGet("history/{accountId:guid}")]
        public async Task<IActionResult> GetHistory(Guid accountId)
        {
            var history = await _context.HandleAsync(accountId);
            return Ok(history);
        }
    }
}
