using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class CreateCheckoutSessionDto
{
    [Required(ErrorMessage = "El correo del cliente es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo del cliente no tiene un formato válido.")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe haber al menos un producto en el checkout.")]
    public List<CartItemDto> Items { get; set; } = new();

    [Required(ErrorMessage = "La URL de éxito es obligatoria.")]
    [Url(ErrorMessage = "La URL de éxito no tiene un formato válido.")]
    public string SuccessUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "La URL de cancelación es obligatoria.")]
    [Url(ErrorMessage = "La URL de cancelación no tiene un formato válido.")]
    public string CancelUrl { get; set; } = string.Empty;
}
