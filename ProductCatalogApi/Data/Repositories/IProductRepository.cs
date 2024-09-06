using ProductCatalogApi.Models;

namespace ProductCatalogApi.Data.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(long id);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductByIdAsync(long id);
        Task<bool> IsProductPresent(string productName);
    }
}
