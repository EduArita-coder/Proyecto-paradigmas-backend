using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IWebHostEnvironment _environment;

    public ProductsController(ProductService productService, IWebHostEnvironment environment)
    {
        _productService = productService;
        _environment = environment;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll(
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null)
    {
        var products = await _productService.GetAllAsync(page, pageSize, search, category);
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("upload")]
    public async Task<ActionResult<object>> UploadImage(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No se recibió ningún archivo." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Formato de imagen no permitido. Usa jpg, jpeg, png o webp." });

        const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
        if (file.Length > maxSizeBytes)
            return BadRequest(new { message = "La imagen no puede pesar más de 5 MB." });

        var fileName = $"{Guid.NewGuid()}{extension}";
        var imagesPath = Path.Combine(_environment.ContentRootPath, "images");
        Directory.CreateDirectory(imagesPath);

        var filePath = Path.Combine(imagesPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"/images/{fileName}";
        return Ok(new { imageUrl });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var product = await _productService.UpdateAsync(id, dto);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}