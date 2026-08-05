using GAMEHOSTING_APIREST.Dtos;

namespace GAMEHOSTING_APIREST.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(int? page = null, int? pageSize = null, string? search = null, string? category = null);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateAsync(Guid id, CreateProductDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}