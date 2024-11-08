using System.ComponentModel.DataAnnotations;

namespace MyStore.Data
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //Щоб можна було зв'язати категорію з кількома товарами
        public ICollection<Product> Product { get; set; }
    }
}
