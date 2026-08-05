using Microsoft.EntityFrameworkCore;
using GAMEHOSTING_APIREST.Database;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Entities;
using GAMEHOSTING_APIREST.Mappers;

namespace GAMEHOSTING_APIREST.Services;

public class TransactionService
{
    private readonly GameHostingDbContext _context;

    public TransactionService(GameHostingDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionDto>> GetAllByUserAsync(string userId)
    {
        var transactions = await _context.Transactions
            .Include(t => t.Product)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(TransactionMapper.ToDto).ToList();
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id, string userId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        return transaction is null ? null : TransactionMapper.ToDto(transaction);
    }

    public async Task<TransactionEntity> CreateAsync(string externalId, decimal amount, string status, string email, Guid productId, string userId)
    {
        var transaction = new TransactionEntity
        {
            Id = Guid.NewGuid(),
            ExternalTransactionId = externalId,
            Amount = amount,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            CustomerEmail = email,
            ProductId = productId,
            UserId = userId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task CreateTransactionsFromCartAsync(string userId, string externalId, string email)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.SessionId == userId)
            .Include(ci => ci.Product)
            .ToListAsync();

        foreach (var item in cartItems)
        {
            var product = item.Product;
            if (product is null) continue;

            for (int i = 0; i < item.Quantity; i++)
            {
                var transaction = new TransactionEntity
                {
                    Id = Guid.NewGuid(),
                    ExternalTransactionId = externalId,
                    Amount = product.Price,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow,
                    CustomerEmail = email,
                    ProductId = product.Id,
                    UserId = userId
                };
                _context.Transactions.Add(transaction);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusByExternalIdAsync(string externalId, string newStatus)
    {
        var transactions = await _context.Transactions
            .Where(t => t.ExternalTransactionId == externalId)
            .ToListAsync();

        foreach (var transaction in transactions)
        {
            transaction.Status = newStatus;
        }

        await _context.SaveChangesAsync();
    }
}
