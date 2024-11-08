using System.ComponentModel.DataAnnotations;

namespace MyStore.Data
{
    public class ProductImage
    {
        [Key]
        public Guid ImageId { get; set; }
        public Guid ProductID { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public Product Product { get; set; }
    }
}
