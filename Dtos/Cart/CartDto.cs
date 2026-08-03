namespace GAMEHOSTING_APIREST.Dtos;

public class CartDto
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}
