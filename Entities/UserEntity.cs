using Microsoft.AspNetCore.Identity;

namespace GAMEHOSTING_APIREST.Entities;

public class UserEntity : IdentityUser
{

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
