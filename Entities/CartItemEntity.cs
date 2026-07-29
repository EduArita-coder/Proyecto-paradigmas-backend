namespace GAMEHOSTING_APIREST.Entities;

public class CartItemEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    // Para agrupar los items del mismo carrito (usuario anónimo o sesión)
    // Ajusta esto si el equipo ya maneja login/usuarios (cámbialo a UserId)
    public string SessionId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación al producto
    public ProductEntity? Product { get; set; }
}