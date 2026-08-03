using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class CartItemDto
{
    [Required] 
    public Guid ProductId { get; set; }

    [Required] 
    [Range(1, 100)] 
    public int Quantity { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}