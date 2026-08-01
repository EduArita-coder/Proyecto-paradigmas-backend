using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class ConfirmPaymentDto
{
    [Required] 
    public string SessionId { get; set; } = string.Empty;
}
