
using ProductCatalogApi.Models;
using ProductCatalogApi.Models.DTOs;
using ProductCatalogApi.Data.Repositories;
using System.Data;

namespace ProductCatalogApi.Services
{
    public class ProductsService
    {
        private readonly IProductRepository _productRepository;

        public ProductsService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
            return await _productRepository.GetProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(long id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} was not found.");
            }

            return product;
        }
        
        public async Task<Product> CreateProductAsync(ProductDtoPlain plainProduct)
        {
            if (await IsProductPresent(plainProduct.Name))
            {
                throw new DuplicateNameException();
            }

            Product product = CreateNewProductFromDto(plainProduct);
            await _productRepository.CreateProductAsync(product);

            return product;
        }

        // optimistic updating approach (possible concurency conflicts!)
        public async Task UpdateProductAsync(Product product, ProductDtoPlain newProduct)
        {
            // if name updates check for duplicate names
            if (product.Name != newProduct.Name && await IsProductPresent(newProduct.Name))
            {
                throw new DuplicateNameException();
            }

            product.Name = newProduct.Name;
            product.Price = newProduct.Price;
            product.Description = newProduct.Description;

            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(long productId)
        {
            await _productRepository.DeleteProductByIdAsync(productId);
        }

        public async Task<bool> IsProductPresent(string productName)
        {
            return await _productRepository.IsProductPresent(productName);
        }

        public async Task<bool> ApplyDiscountAsync(long productId, int discountPercentage)
        {
            if (discountPercentage <= 0 || discountPercentage > 100)
            {
                return false;
            }

            var product = await GetProductByIdAsync(productId);

            product.Price = product.Price * (1 - ((decimal)discountPercentage) / 100);
            await _productRepository.UpdateProductAsync(product);

            return true;
        }
    }
}