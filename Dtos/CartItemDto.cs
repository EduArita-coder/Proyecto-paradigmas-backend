namespace GAMEHOSTING_APIREST.Dtos;

public class CartItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}