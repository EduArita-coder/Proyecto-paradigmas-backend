using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services.Interfaces;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/carrito")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] CartItemDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        if (userId is null) return Unauthorized();

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

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartDto>> UpdateItemQuantity(Guid productId, [FromBody] int quantity)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

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

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid productId)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var cart = await _cartService.RemoveItemFromCartAsync(userId, productId);
        return Ok(cart);
    }

    [HttpDelete]
    public async Task<ActionResult<CartDto>> ClearCart()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var cart = await _cartService.ClearCartAsync(userId);
        return Ok(cart);
    }

    [HttpGet("validate")]
    public async Task<ActionResult<CartDto>> ValidateCart()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var cart = await _cartService.ValidateCartAsync(userId);
        return Ok(cart);
    }

    [HttpGet("totals")]
    public async Task<ActionResult<CartDto>> CalculateTotals()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var cart = await _cartService.CalculateTotalsAsync(userId);
        return Ok(cart);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CartDto>> Checkout()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

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

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}