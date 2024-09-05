using Microsoft.AspNetCore.Mvc;
using ProductCatalogApi.Models;
using ProductCatalogApi.Models.DTOs;
using ProductCatalogApi.Services;

namespace ProductCatalogApi.Controllers
{
    [Route("productCatalogApi/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;
        private readonly ILogger<ProductsController> _logger;


        public ProductsController(ProductsService productsService, ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        // productCatalogApi/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(long id)
        {
            var product = await _productsService.GetProductByIdAsync(id);

            if (product != null)
            {
                return Ok(product);
            }

            return HandleProductNotFound(id);
        }

        // productCatalogApi/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductDtoPlain plainProduct)
        {
            if (await _productsService.IsProductPresent(plainProduct.Name))
            {
                return Conflict(new { message = "A product with the same name already exists." });
            }

            Product product = _productsService.CreateNewProductFromDto(plainProduct);

            await _productsService.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // productCatalogApi/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, ProductDtoPlain newProduct)
        {
            var product = await _productsService.GetProductByIdAsync(id);

            if (product != null)
            {
                await _productsService.UpdateProductAsync(product, newProduct);

                return NoContent();
            }

            return HandleProductNotFound(id);
        }

        // productCatalogApi/Products/get{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = await _productsService.GetProductByIdAsync(id);

            if (product != null)
            {
                await _productsService.DeleteProductAsync(product);
                return NoContent();
            }

            return HandleProductNotFound(id);
        }

        // productCatalogApi/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok(await _productsService.GetProductsAsync());
        }


        private ActionResult HandleProductNotFound(long id)
        {
            _logger.LogWarning($"Product with ID {id} not found.");
            return NotFound();
        }
    }
}
