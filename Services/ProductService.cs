using GAMEHOSTING_APIREST.Database;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Mappers;
using Microsoft.EntityFrameworkCore;

namespace GAMEHOSTING_APIREST.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
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
}
