using System.Data;
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
            try
            {
                var product = await _productsService.GetProductByIdAsync(id);
                return Ok(product);

            }
            catch (KeyNotFoundException)
            {
                return HandleProductNotFound(id);

            }
        }

        // productCatalogApi/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductDtoPlain plainProduct)
        {
            try
            {
                var newProduct = await _productsService.CreateProductAsync(plainProduct);
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
            }
            catch (DuplicateNameException)
            {
                return HandleDuplicateProductName(plainProduct.Name);
            }
        }

        // productCatalogApi/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, ProductDtoPlain newProduct)
        {
            try
            {
                var product = await _productsService.GetProductByIdAsync(id);
                try
                {
                    await _productsService.UpdateProductAsync(product, newProduct);
                }
                catch (DuplicateNameException)
                {
                    return HandleDuplicateProductName(newProduct.Name);
                }

                return NoContent();

            }
            catch (KeyNotFoundException)
            {
                return HandleProductNotFound(id);
            }
        }

        // productCatalogApi/Products/get{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                var product = await _productsService.GetProductByIdAsync(id);
                await _productsService.DeleteProductAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return HandleProductNotFound(id);

            }
        }

        // productCatalogApi/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok(await _productsService.GetProductsAsync());
        }

        // productCatalogApi/Products/discount/{id}/{percentage}
        [HttpPut("discount/{id}/{percentage}")]
        public async Task<ActionResult<Product>> DiscnoutProductPrice(long id, int percentage)
        {
            try
            {
                var product = await _productsService.GetProductByIdAsync(id);
                bool wasDiscounted = await _productsService.ApplyDiscountAsync(id, percentage);

                if (wasDiscounted)
                {
                    return Ok(await _productsService.GetProductByIdAsync(id));
                }

                return BadRequest(new {Message = "Percentage exceeds min/max value [1-100]."});
            }
            catch (KeyNotFoundException)
            {
                return HandleProductNotFound(id);
            }
        }

        private ActionResult HandleProductNotFound(long id)
        {
            _logger.LogWarning($"Product with ID {id} not found.");
            return NotFound(new {Message = $"Product with ID {id} is not present in catalog."});
        }

        private ActionResult HandleDuplicateProductName(string name)
        {
            _logger.LogWarning($"Product with name {name} already present.");
            return Conflict(new {Message =  $"A product with the name {name} already exists in the catalog."});
        }
    }
}
