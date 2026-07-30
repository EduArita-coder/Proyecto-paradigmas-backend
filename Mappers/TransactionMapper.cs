using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Entities;

namespace GAMEHOSTING_APIREST.Mappers;

public static class TransactionMapper
{
    public static TransactionDto ToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            ExternalTransactionId = transaction.ExternalTransactionId,
            Amount = transaction.Amount,
            Status = transaction.Status,
            CreatedAt = transaction.CreatedAt,
            CustomerEmail = transaction.CustomerEmail,
            ProductName = transaction.Product?.Name ?? string.Empty
        };
    }
}
