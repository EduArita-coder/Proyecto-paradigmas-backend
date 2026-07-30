using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Entities;

namespace GAMEHOSTING_APIREST.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Cpu = product.Cpu,
            Ram = product.Ram,
            Slots = product.Slots
        };
    }

    public static Product ToEntity(CreateProductDto dto)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            Cpu = dto.Cpu,
            Ram = dto.Ram,
            Slots = dto.Slots
        };
    }
}
