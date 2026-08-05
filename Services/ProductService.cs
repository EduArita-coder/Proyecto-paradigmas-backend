using GAMEHOSTING_APIREST.Database;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Mappers;
using GAMEHOSTING_APIREST.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GAMEHOSTING_APIREST.Services;

public class ProductService : IProductService
{
    private readonly GameHostingDbContext _context;

    public ProductService(GameHostingDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Select(ProductMapper.ToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        return product is null ? null : ProductMapper.ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = ProductMapper.ToEntity(dto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return ProductMapper.ToDto(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl;
        product.Cpu = dto.Cpu;
        product.Ram = dto.Ram;
        product.Slots = dto.Slots;

        await _context.SaveChangesAsync();
        return ProductMapper.ToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
