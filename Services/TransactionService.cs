using Microsoft.EntityFrameworkCore;
using GAMEHOSTING_APIREST.Database;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Entities;
using GAMEHOSTING_APIREST.Mappers;

namespace GAMEHOSTING_APIREST.Services;

public class TransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionDto>> GetAllAsync()
    {
        var transactions = await _context.Transactions
            .Include(t => t.Product)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(TransactionMapper.ToDto).ToList();
    }

    public async Task<Transaction> CreateAsync(string externalId, decimal amount, string status, string email, Guid productId)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ExternalTransactionId = externalId,
            Amount = amount,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            CustomerEmail = email,
            ProductId = productId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task UpdateStatusBySessionIdAsync(string sessionId, string newStatus)
    {
        var transactions = await _context.Transactions
            .Where(t => t.ExternalTransactionId == sessionId)
            .ToListAsync();

        foreach (var transaction in transactions)
        {
            transaction.Status = newStatus;
        }

        await _context.SaveChangesAsync();
    }
}
