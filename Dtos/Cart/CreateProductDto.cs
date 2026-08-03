using System.ComponentModel.DataAnnotations;

namespace GAMEHOSTING_APIREST.Dtos;

public class CreateProductDto
{
    [Required] 
    public string Name { get; set; } = string.Empty;

    [Required] 
    public string Description { get; set; } = string.Empty;

    [Required] 
    [Range(0.01, 10000)] 
    public decimal Price { get; set; }

    [Required] 
    public string ImageUrl { get; set; } = string.Empty;

    [Required] 
    public string Cpu { get; set; } = string.Empty;

    [Required] 
    public string Ram { get; set; } = string.Empty;

    [Required] 
    [Range(1, 1000)] 
    public int Slots { get; set; }
}
