namespace GAMEHOSTING_APIREST.Dtos;

public class PayPalOrderResponseDto
{
    public string OrderId { get; set; } = string.Empty;
    public string ApprovalUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
