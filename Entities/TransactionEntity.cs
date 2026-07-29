namespace GAMEHOSTING_APIREST.Entities;

public class TransactionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ExternalTransactionId { get; set; } = string.Empty; // Id que devuelve Stripe
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // Pendiente/Exitoso/Fallido
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CustomerEmail { get; set; } = string.Empty;

    // FK en vez de guardar el nombre repetido (mejor práctica, evita datos duplicados)
    public Guid ProductId { get; set; }
    public ProductEntity? Product { get; set; }
}