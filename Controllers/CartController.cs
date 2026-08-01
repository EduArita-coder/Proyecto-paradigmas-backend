using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services.Interfaces;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<CartDto>> GetCart(string userId)
    {
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("{userId}/items")]
    public async Task<ActionResult<CartDto>> AddItem(string userId, CartItemDto dto)
    {
        try
        {
            var cart = await _cartService.AddItemToCartAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{userId}/items/{productId}")]
    public async Task<ActionResult<CartDto>> UpdateItemQuantity(string userId, Guid productId, [FromBody] int quantity)
    {
        try
        {
            var cart = await _cartService.UpdateItemQuantityAsync(userId, productId, quantity);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveItem(string userId, Guid productId)
    {
        var cart = await _cartService.RemoveItemFromCartAsync(userId, productId);
        return Ok(cart);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult<CartDto>> ClearCart(string userId)
    {
        var cart = await _cartService.ClearCartAsync(userId);
        return Ok(cart);
    }

    [HttpGet("{userId}/validate")]
    public async Task<ActionResult<CartDto>> ValidateCart(string userId)
    {
        var cart = await _cartService.ValidateCartAsync(userId);
        return Ok(cart);
    }

    [HttpGet("{userId}/totals")]
    public async Task<ActionResult<CartDto>> CalculateTotals(string userId)
    {
        var cart = await _cartService.CalculateTotalsAsync(userId);
        return Ok(cart);
    }

    [HttpPost("{userId}/checkout")]
    public async Task<ActionResult<CartDto>> Checkout(string userId)
    {
        try
        {
            var cart = await _cartService.CheckoutAsync(userId);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}