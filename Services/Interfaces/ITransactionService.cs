using GAMEHOSTING_APIREST.Dtos;

namespace GAMEHOSTING_APIREST.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, string userId);
        Task<TransactionDto> GetTransactionByIdAsync(Guid id, string userId);
        Task<List<TransactionDto>> GetUserTransactionsAsync(string userId);
        Task<List<TransactionDto>> GetAllTransactionsAsync();
    }
}