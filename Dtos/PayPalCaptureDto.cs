using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class PayPalCaptureDto
{
    [Required]
    public string OrderId { get; set; } = string.Empty;
}

public class PayPalCaptureResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
