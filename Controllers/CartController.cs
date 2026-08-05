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
    private readonly IPaypalService _paypalService;
    private readonly IConfiguration _configuration;

    public CartController(ICartService cartService, IPaypalService paypalService, IConfiguration configuration)
    {
        _cartService = cartService;
        _paypalService = paypalService;
        _configuration = configuration;
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

    /// <summary>
    /// Inicia el proceso de pago: valida el carrito, crea una orden en PayPal
    /// y devuelve la URL de aprobación para redirigir al usuario.
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult> Checkout()
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        try
        {
            // Validar que el carrito tenga productos
            var cart = await _cartService.CheckoutAsync(userId);

            // Leer URLs de configuración
            var successUrl = _configuration["PayPal:SuccessUrl"] ?? "http://localhost:5173/checkout/success";
            var cancelUrl  = _configuration["PayPal:CancelUrl"]  ?? "http://localhost:5173/checkout/cancel";

            // Crear la orden en PayPal Sandbox
            var paypalOrder = await _paypalService.CreateOrderAsync(cart, successUrl, cancelUrl);

            // Devolver approvalUrl y orderId al frontend
            return Ok(new
            {
                approvalUrl = paypalOrder.ApprovalUrl,
                orderId     = paypalOrder.OrderId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear la orden de pago.", detail = ex.Message });
        }
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}