using GAMEHOSTING_APIREST.Dtos;

namespace GAMEHOSTING_APIREST.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddItemToCartAsync(string userId, Guid productId, int quantity);
        Task<CartDto> RemoveItemFromCartAsync(string userId, Guid productId);
        Task<CartDto> UpdateItemQuantityAsync(string userId, Guid productId, int quantity);
        Task<CartDto> ClearCartAsync(string userId);
        Task<CartDto> CheckoutAsync(string userId);
        Task<CartDto> ValidateCartAsync(string userId);
        Task<CartDto> CalculateTotalsAsync(string userId);

    }
}