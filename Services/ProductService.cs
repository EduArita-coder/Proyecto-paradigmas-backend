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

    public async Task<List<ProductDto>> GetAllAsync(int? page = null, int? pageSize = null, string? search = null, string? category = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Description.ToLower().Contains(term));
        }

        // No hay un campo "Category" dedicado en ProductEntity todavia;
        // por ahora se filtra por coincidencia en el nombre (ej. "minecraft", "rust", "cs2").
        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryTerm = category.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(categoryTerm));
        }

        query = query.OrderBy(p => p.Name);

        // La paginacion es opcional: si no se manda page/pageSize, se devuelve
        // el listado completo (filtrado), igual que antes, sin romper el contrato
        // de respuesta que ya usa el frontend (un array plano de productos).
        if (page.HasValue && pageSize.HasValue)
        {
            var currentPage = page.Value < 1 ? 1 : page.Value;
            var size = pageSize.Value < 1 ? 6 : Math.Min(pageSize.Value, 50);
            query = query.Skip((currentPage - 1) * size).Take(size);
        }

        var products = await query.ToListAsync();
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

    public Task<List<ProductDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}