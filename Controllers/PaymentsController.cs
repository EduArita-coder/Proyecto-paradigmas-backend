using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services;
using GAMEHOSTING_APIREST.Services.Interfaces;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/pagos")]
[Authorize]
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

    /// <summary>
    /// Endpoint principal de captura que llama el frontend:
    /// POST /api/pagos/capture
    /// Body: { "token": "ORDER_ID_FROM_PAYPAL", "payerId": "PAYER_ID" }
    /// </summary>
    [HttpPost("capture")]
    public async Task<ActionResult> CaptureByToken([FromBody] PayPalTokenCaptureDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return await ProcessCapture(dto.Token, userId);
    }

    /// <summary>
    /// Endpoint alternativo (fallback): GET /api/pagos/success?token=...&PayerID=...
    /// Algunos flujos de PayPal Sandbox redirigen directamente aquí.
    /// </summary>
    [HttpGet("success")]
    public async Task<ActionResult> SuccessCallback([FromQuery] string token, [FromQuery] string? PayerID)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token de pago no proporcionado." });

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return await ProcessCapture(token, userId);
    }

    /// <summary>
    /// Crea una orden PayPal manualmente (uso interno / testing).
    /// POST /api/pagos/create-order
    /// </summary>
    [HttpPost("create-order")]
    public async Task<ActionResult<PayPalOrderResponseDto>> CreateOrder([FromBody] CreateCheckoutSessionDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var cart = await _cartService.ValidateCartAsync(userId);
        if (cart.Items.Count == 0)
            return BadRequest(new { message = "El carrito está vacío." });

        var order = await _paypalService.CreateOrderAsync(cart, dto.SuccessUrl, dto.CancelUrl);
        return Ok(order);
    }

    /// <summary>
    /// Captura una orden PayPal por su OrderId (uso interno / testing).
    /// POST /api/pagos/capture-order
    /// </summary>
    [HttpPost("capture-order")]
    public async Task<ActionResult> CaptureOrder([FromBody] PayPalCaptureDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return await ProcessCapture(dto.OrderId, userId);
    }

    /// <summary>
    /// Cancela el flujo de pago. No hace nada en la DB, solo confirma al frontend.
    /// POST /api/pagos/cancel
    /// </summary>
    [HttpPost("cancel")]
    public ActionResult CancelOrder()
    {
        return Ok(new { message = "Pago cancelado." });
    }

    // ──────────────────────────────────────────────────────────────────
    // Lógica compartida: captura el pago en PayPal, registra
    // transacciones en la base de datos y limpia el carrito.
    // ──────────────────────────────────────────────────────────────────
    private async Task<ActionResult> ProcessCapture(string orderId, string userId)
    {
        try
        {
            // 1. Capturar el pago en PayPal
            var captureResult = await _paypalService.CaptureOrderAsync(orderId);

            if (!captureResult.Success)
                return BadRequest(new { message = captureResult.Message });

            // 2. Obtener email del pagador (si PayPal no lo devuelve, usar el del token JWT)
            var customerEmail = !string.IsNullOrEmpty(captureResult.Email)
                ? captureResult.Email
                : User.FindFirstValue(System.Security.Claims.ClaimTypes.Email) ?? "cliente@gamehosting.com";

            // 3. Registrar una transacción por cada item del carrito
            await _transactionService.CreateTransactionsFromCartAsync(userId, captureResult.OrderId, customerEmail);

            // 4. Limpiar el carrito
            await _cartService.ClearCartAsync(userId);

            return Ok(new
            {
                message = "Pago procesado exitosamente.",
                orderId = captureResult.OrderId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al procesar el pago.", detail = ex.Message });
        }
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
    }
}
