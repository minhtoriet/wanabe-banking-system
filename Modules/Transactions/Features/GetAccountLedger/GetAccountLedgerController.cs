using FluentMigrator;
using Microsoft.AspNetCore.Mvc;

namespace Transactions.Features.GetAccountLedger
{
    [ApiController]
    [Route("api/transactions")]
    [Tags("Transactions")]
    public class GetAccountLedgerController : ControllerBase
    {
        private readonly IGetAccountLedgerHandler _handler;

        public GetAccountLedgerController(IGetAccountLedgerHandler handler)
        {
            _handler = handler;
        }
        [HttpGet("ledger/{accountId:guid}")]
        public async Task<IActionResult> GetLedger(Guid accountId)
        {
            var ledger = await _handler.HandleAsync(accountId);
            return Ok(ledger);
        }
    }
}
