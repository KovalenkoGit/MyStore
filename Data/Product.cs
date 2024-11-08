using System.ComponentModel.DataAnnotations;

namespace MyStore.Data
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal Price { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        //Все що стосується товару (Фото товару, Деталі покупки)
        public ICollection<ProductImage> ProductImages { get; set; }
    }
}
