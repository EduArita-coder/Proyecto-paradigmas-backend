using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class RegisterDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(255, ErrorMessage = "El correo supera el máximo permitido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [StringLength(100, ErrorMessage = "La contraseña supera el máximo permitido.")]
    public string Password { get; set; } = string.Empty;
}
