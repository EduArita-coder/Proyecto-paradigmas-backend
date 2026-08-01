using GAMEHOSTING_APIREST.Database;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Entities;
using GAMEHOSTING_APIREST.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GAMEHOSTING_APIREST.Services
{
    public class CartService : ICartService
    {
        private readonly GameHostingDbContext _context;

        public CartService(GameHostingDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> AddItemToCartAsync(string userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            var product = await _context.Products.FindAsync(productId);
            if (product is null)
                throw new KeyNotFoundException($"Producto con id {productId} no encontrado.");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.SessionId == userId && ci.ProductId == productId);

            if (existingItem is not null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItemEntity
                {
                    SessionId = userId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> RemoveItemFromCartAsync(string userId, Guid productId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.SessionId == userId && ci.ProductId == productId);

            if (item is not null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> UpdateItemQuantityAsync(string userId, Guid productId, int quantity)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.SessionId == userId && ci.ProductId == productId);

            if (item is null)
                throw new KeyNotFoundException("El producto no esta en el carrito.");

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> ClearCartAsync(string userId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.SessionId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> ValidateCartAsync(string userId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.SessionId == userId)
                .ToListAsync();

            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var existingProductIds = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var invalidItems = items.Where(i => !existingProductIds.Contains(i.ProductId)).ToList();

            if (invalidItems.Count > 0)
            {
                _context.CartItems.RemoveRange(invalidItems);
                await _context.SaveChangesAsync();
            }

            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> CalculateTotalsAsync(string userId)
        {
            return await BuildCartDtoAsync(userId);
        }

        public async Task<CartDto> CheckoutAsync(string userId)
        {
            var cart = await ValidateCartAsync(userId);

            if (cart.Items.Count == 0)
                throw new InvalidOperationException("El carrito esta vacio, no se puede procesar el checkout.");
            return cart;
        }

        private async Task<CartDto> BuildCartDtoAsync(string userId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.SessionId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();

            var itemDtos = items.Select(ci => new CartItemDto
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                ProductName = ci.Product?.Name ?? string.Empty,
                UnitPrice = ci.Product?.Price ?? 0m,
                Subtotal = (ci.Product?.Price ?? 0m) * ci.Quantity
            }).ToList();

            return new CartDto
            {
                UserId = userId,
                Items = itemDtos,
                TotalAmount = itemDtos.Sum(i => i.Subtotal)
            };
        }
    }
}