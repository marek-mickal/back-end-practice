
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogApi.Models.DTOs
{
    public class ProductDtoPlain
    {
        [Required, StringLength(50)]
        public required string Name { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price exeeds minimal/maximal value")]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
