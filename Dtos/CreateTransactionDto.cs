namespace GAMEHOSTING_APIREST.Dtos;

public class CreateTransactionDto
{
    public string ExternalTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
}