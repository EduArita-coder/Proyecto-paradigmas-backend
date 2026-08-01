using Microsoft.AspNetCore.Mvc;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services;

namespace GAMEHOSTING_APIREST.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
