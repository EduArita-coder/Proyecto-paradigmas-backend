namespace GAMEHOSTING_APIREST.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public string ExternalTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
