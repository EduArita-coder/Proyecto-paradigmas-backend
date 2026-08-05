using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<UserEntity> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "El correo electrónico ya está registrado." });
        }

        var user = new UserEntity
        {
            UserName = dto.UserName,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { message = "Error al crear el usuario.", errors });
        }

        // Todo usuario que se registra normalmente entra como "Cliente".
        // El rol "Admin" solo se asigna manualmente (o via el usuario admin sembrado en Program.cs).
        await _userManager.AddToRoleAsync(user, "Cliente");

        var response = await GenerateJwtTokenAsync(user);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        var response = await GenerateJwtTokenAsync(user);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { user.Id, user.UserName, user.Email, roles });
    }

    private async Task<AuthResponseDto> GenerateJwtTokenAsync(UserEntity user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("Key") ?? "SuperSecretKeyForGameHostingApiRest2026_MustBe32CharsLong!";
        var issuer = jwtSettings.GetValue<string>("Issuer") ?? "GAMEHOSTING_APIREST";
        var audience = jwtSettings.GetValue<string>("Audience") ?? "GAMEHOSTING_APIREST";
        var expireMinutes = jwtSettings.GetValue<int>("ExpireMinutes", 1440);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Un claim de rol por cada rol del usuario. Esto es lo que permite
        // que [Authorize(Roles = "Admin")] funcione en los controllers.
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            ExpiresAt = expiresAt
        };
    }
}