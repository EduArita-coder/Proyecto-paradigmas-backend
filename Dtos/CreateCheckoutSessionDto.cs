namespace GAMEHOSTING_APIREST.Dtos;

public class CreateCheckoutSessionDto
{
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
