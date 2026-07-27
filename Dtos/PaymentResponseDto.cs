namespace GAMEHOSTING_APIREST.Dtos;

public class PaymentResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
}
