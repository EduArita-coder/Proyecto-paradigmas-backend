using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "La contraseña no puede estar vacía.")]
    public string Password { get; set; } = string.Empty;
}
