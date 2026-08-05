using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services;
using GAMEHOSTING_APIREST.Services.Interfaces;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/pagos")]
[Authorize(Policy = "ClienteOnly")]
public class PaymentsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IPaypalService _paypalService;
    private readonly TransactionService _transactionService;

    public PaymentsController(ICartService cartService, IPaypalService paypalService, TransactionService transactionService)
    {
        _cartService = cartService;
        _paypalService = paypalService;
        _transactionService = transactionService;
    }

    [HttpPost("create-order")]
    public async Task<ActionResult<PayPalOrderResponseDto>> CreateOrder(CreateCheckoutSessionDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var cart = await _cartService.ValidateCartAsync(userId);
        if (cart.Items.Count == 0)
            return BadRequest("El carrito está vacío.");

        var order = await _paypalService.CreateOrderAsync(cart, dto.SuccessUrl, dto.CancelUrl);
        return Ok(order);
    }

    [HttpPost("capture-order")]
    public async Task<ActionResult> CaptureOrder(PayPalCaptureDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var captureResult = await _paypalService.CaptureOrderAsync(dto.OrderId);
        if (!captureResult.Success)
            return BadRequest(captureResult.Message);

        var customerEmail = !string.IsNullOrEmpty(captureResult.Email) ? captureResult.Email : "cliente@example.com";
        await _transactionService.CreateTransactionsFromCartAsync(userId, captureResult.OrderId, customerEmail);
        await _cartService.ClearCartAsync(userId);

        return Ok(new { message = "Pago capturado exitosamente.", orderId = dto.OrderId });
    }

    [HttpPost("cancel")]
    public async Task<ActionResult> CancelOrder(PayPalCaptureDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(new { message = "Pago cancelado.", orderId = dto.OrderId });
    }
}
