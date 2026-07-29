using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly TransactionService _transactionService;

    public PaymentsController(ProductService productService, TransactionService transactionService)
    {
        _productService = productService;
        _transactionService = transactionService;
    }

    [HttpPost("create-checkout-session")]
    public async Task<ActionResult<PaymentResponseDto>> CreateCheckoutSession(CreateCheckoutSessionDto dto)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            return BadRequest("El carrito esta vacio.");

        // Verificar que todos los productos existan
        foreach (var item in dto.Items)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product is null)
                return NotFound($"Producto con id {item.ProductId} no encontrado.");
        }

        // Generar un ID de sesion simulado (sera reemplazado por Stripe)
        var sessionId = Guid.NewGuid().ToString();

        // Crear transacciones pendientes por cada item del carrito
        foreach (var item in dto.Items)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product is null) continue;

            for (int i = 0; i < item.Quantity; i++)
            {
                await _transactionService.CreateAsync(
                    externalId: sessionId,
                    amount: product.Price,
                    status: "Pending",
                    email: dto.CustomerEmail,
                    productId: item.ProductId
                );
            }
        }

        return Ok(new PaymentResponseDto
        {
            SessionId = sessionId,
            PaymentUrl = dto.SuccessUrl
        });
    }

    [HttpPost("confirm")]
    public async Task<ActionResult> ConfirmPayment(ConfirmPaymentDto dto)
    {
        if (string.IsNullOrEmpty(dto.SessionId))
            return BadRequest("SessionId es requerido.");

        var transactions = await _transactionService.GetAllAsync();
        var pending = transactions.Where(t => t.ExternalTransactionId == dto.SessionId && t.Status == "Pending").ToList();

        if (pending.Count == 0)
            return NotFound("No se encontraron transacciones pendientes para esta sesion.");

        await _transactionService.UpdateStatusBySessionIdAsync(dto.SessionId, "Completed");

        return Ok(new { message = "Pago confirmado exitosamente.", sessionId = dto.SessionId });
    }

    [HttpPost("cancel")]
    public async Task<ActionResult> CancelPayment(ConfirmPaymentDto dto)
    {
        if (string.IsNullOrEmpty(dto.SessionId))
            return BadRequest("SessionId es requerido.");

        await _transactionService.UpdateStatusBySessionIdAsync(dto.SessionId, "Canceled");

        return Ok(new { message = "Pago cancelado.", sessionId = dto.SessionId });
    }
}
