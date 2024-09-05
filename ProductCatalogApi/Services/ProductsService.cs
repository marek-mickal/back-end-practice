
using ProductCatalogApi.Models;
using ProductCatalogApi.Models.DTOs;
using ProductCatalogApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalogApi.Services
{
    public class ProductsService
    {

        private readonly AppDbContext _dbContext;

        public ProductsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Product CreateNewProductFromDto(ProductDtoPlain dto)
        {
            return new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description
            };
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(long id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        // optimistic updating approach (possible concurency conflicts!)
        public async Task UpdateProductAsync(Product product, ProductDtoPlain newProduct)
        {
            product.Name = newProduct.Name;
            product.Price = newProduct.Price;
            product.Description = newProduct.Description;

            _dbContext.Entry(product).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsProductPresent(string productName)
        {
            return await _dbContext.Products.AnyAsync(p => p.Name == productName);
        }

    }
}