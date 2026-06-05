using Accounts.Controllers;
using Microsoft.EntityFrameworkCore;
using Transactions.DTOs;
using Transactions.Models;
using Transactions.Models.Context;

namespace Transactions.Features.TransferMoney
{
    internal class TransferMoneyHandler : ITransferMoneyHandler
    {
        private readonly TransactionManagementContext _context;
        private readonly IAccountService _accountService;
        public TransferMoneyHandler(TransactionManagementContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }
        public async Task<TransferResultDto> HandleAsync(CreateTransferRequestDto request)
        {
            //check IDEMPOTENCY
            var existingOrder = await _context.PaymentOrders.AsNoTracking().FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey);
            if (existingOrder != null)
            {
                return new TransferResultDto(
                IsSuccess: existingOrder.Status == Status.Executed,
                Message: $"Duplicate transaction. Current status: {existingOrder.Status}",
                PaymentId: existingOrder.PaymentId,
                Status: existingOrder.Status.ToString()
                 );
            }

            //initialize payment order
            var order = new PaymentOrder
            {
                PaymentId = Guid.NewGuid(),
                IdempotencyKey = request.IdempotencyKey,
                DebtorAccountId = request.DebtorAccountId,
                CreditorAccountId = request.CreditorAccountId,
                Amount = request.Amount,
                Status = Status.Initiated,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentOrders.Add(order);
            await _context.SaveChangesAsync();



            //check balance
            order.Status = Status.Processing;
            await _context.SaveChangesAsync();
            bool hasEnoughMoney = await _accountService.HasSufficientBalanceAsync(request.DebtorAccountId, request.Amount);

            if (!hasEnoughMoney)
            {
                order.Status = Status.Failed;
                await _context.SaveChangesAsync();
                return new TransferResultDto(false, "Failure: Your account has insufficient balance.", order.PaymentId, "Failed");
            }
            //proceed with ledger
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var debitEntry = new LedgerEntry
                {
                    EntryId = Guid.NewGuid(),
                    PaymentId = order.PaymentId,
                    AccountId = request.DebtorAccountId,
                    TransactionType = TransactionType.Debit,
                    Amount = request.Amount,
                    CreatedAt = DateTime.UtcNow
                };

                var creditEntry = new LedgerEntry
                {
                    EntryId = Guid.NewGuid(),
                    PaymentId = order.PaymentId,
                    AccountId = request.CreditorAccountId,
                    TransactionType = TransactionType.Credit,
                    Amount = request.Amount,
                    CreatedAt = DateTime.UtcNow
                };

                _context.LedgerEntries.AddRange(debitEntry, creditEntry);

                order.Status = Status.Executed;
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return new TransferResultDto(true, "Money transfer successful!", order.PaymentId, "Executed");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                order.Status = Status.Failed;
                await _context.SaveChangesAsync();
                return new TransferResultDto(false, $"System error: {ex.Message}", order.PaymentId, "Failed");
            }
        }
    }
}
