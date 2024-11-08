using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Models
{
    public class ProductModel
    {
        public Guid ProductId { get; set; }
        [StringLength(100, MinimumLength = 5)]
        [Required(ErrorMessage = "Введіть назву")]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        public Guid? CategoryId { get; set; }
        [ValidateNever]
        public string Category { get; set; }
        //[Required]
        [ValidateNever]
        public IFormFile CoverPhoto { get; set; }
        [ValidateNever]
        public string CoverImageUrl { get; set; }
        [Display(Name = "Виберіть галерею зображень для товару")]
        //[Required]
        [ValidateNever]
        public IFormFileCollection GalleryFiles { get; set; }
        [ValidateNever]
        public List<GalleryModel> Gallery { get; set; }
    }
}
