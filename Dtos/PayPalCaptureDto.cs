using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

// DTO para capturar por OrderId (endpoint anterior)
public class PayPalCaptureDto
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
}

// DTO para capturar usando el token que PayPal envía en la URL de retorno
// PayPal redirige a: /checkout/success?token=ORDER_ID&PayerID=PAYER_ID
public class PayPalTokenCaptureDto
{
    [Required]
    public string Token { get; set; } = string.Empty; // Token = OrderId en PayPal Sandbox

    public string? PayerId { get; set; }
}

public class PayPalCaptureResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
