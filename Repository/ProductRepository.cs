using MyStore.Models;
using MyStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using NuGet.Packaging;

namespace MyStore.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context = null;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
		public async Task<Guid> AddNewProduct(ProductModel product)
		{
			var newProduct = new Product()
			{
				Name = product.Name,
				Description = product.Description,
				Price = product.Price,
				CategoryId = product.CategoryId,
				DateAdded = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				CoverImageUrl = product.CoverImageUrl ?? "/images/default-cover.png"
			};

			// Ініціалізуємо ProductImages з пустим списком
			newProduct.ProductImages = new List<ProductImage>();

			// Додаємо фото з галереї, якщо є файли
			if (product.Gallery != null && product.Gallery.Any())
			{
				newProduct.ProductImages.AddRange(product.Gallery.Select(image => new ProductImage()
				{
					Name = image.Name,
					ImageURL = image.ImageURL
				}));
			}
			else
			{
				// Додаємо фото за замовчуванням, якщо галерея порожня
				newProduct.ProductImages.Add(new ProductImage()
				{
					Name = "Default Image",
					ImageURL = "/images/nophoto.png" // Шлях до зображення за замовчуванням
                });
			}

			await _context.Product.AddAsync(newProduct);
			await _context.SaveChangesAsync();
			return newProduct.ProductId;
		}
		public async Task<List<ProductModel>> GetAllProduct()
        {
            return await _context.Product
                  .Select(product => new ProductModel()
                  {
                      ProductId = product.ProductId,
                      Name = product.Name,
                      Description = product.Description,
                      Price = product.Price,
                      CategoryId = product.CategoryId,
                      Category = product.Category.Name,
                      DateAdded = product.DateAdded,
                      DateUpdated = product.DateUpdated,
                      CoverImageUrl = product.CoverImageUrl
                  }).ToListAsync();
        }
        public async Task<List<ProductModel>> GetTopProduct(int count)
        {
            return await _context.Product
                  .Select(product => new ProductModel()
                  {
                      ProductId = product.ProductId,
                      Name = product.Name,
                      Description = product.Description,
                      Price = product.Price,
                      CategoryId = product.CategoryId,
                      Category = product.Category.Name,
                      DateAdded = product.DateAdded,
                      DateUpdated = product.DateUpdated,
                      CoverImageUrl = product.CoverImageUrl
                  }).OrderBy(x => Guid.NewGuid()).Take(count).ToListAsync() ?? new List<ProductModel>();
        }
        public async Task<ProductModel?> GetProductById(Guid productId)
        {
            return await _context.Product
                .Where(x => x.ProductId == productId)
                .Select(product => new ProductModel()
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    Category = product.Category != null ? product.Category.Name : "Без категорії",
                    DateAdded = product.DateAdded,
                    DateUpdated = product.DateUpdated,
                    CoverImageUrl = product.CoverImageUrl,
                    Gallery = product.ProductImages != null ? product.ProductImages.Select(g => new GalleryModel()
                    {
                        ImageId = g.ImageId,
                        Name = g.Name,
                        ImageURL = g.ImageURL
                    }).ToList() : new List<GalleryModel>(),
                }).FirstOrDefaultAsync();
        }
        public async Task<List<ProductModel>> GetProductByCategoryId(Guid categoryId)
        {
            return await _context.Product
                .Where(x => x.CategoryId == categoryId)
                .Select(product => new ProductModel()
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    Category = product.Category.Name,
                    DateAdded = product.DateAdded,
                    DateUpdated = product.DateUpdated,
                    CoverImageUrl = product.CoverImageUrl
                }).ToListAsync();
        }
        public async Task<List<ProductModel>> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                // Якщо пошуковий запит порожній, повертаємо всі товари
                return await _context.Product
                    .Select(product => new ProductModel
                    {
                        ProductId = product.ProductId,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        CoverImageUrl = product.CoverImageUrl
                    }).ToListAsync();
            }

            // Фільтрація товарів за ключовим словом з урахуванням регістру
            var searchName = searchTerm.ToLower();

            return await _context.Product
                .Where(p => p.Name.ToLower().Contains(searchName) ||
                            p.Description.ToLower().Contains(searchName))
                .Select(product => new ProductModel
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CoverImageUrl = product.CoverImageUrl
                }).ToListAsync();
        }
        public async Task<List<GalleryModel>> GetExistingGallery(Guid productId)
        {
            var product = await _context.Product
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            return product?.ProductImages
                .Select(pi => new GalleryModel { Name = pi.Name, ImageURL = pi.ImageURL })
                .ToList();
        }
        public async Task<bool> UpdateProduct(ProductModel productModel)
        {
            var existingProduct = await _context.Product
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productModel.ProductId);

            if (existingProduct == null) return false;

            existingProduct.Name = productModel.Name;
            existingProduct.Description = productModel.Description;
            existingProduct.Price = productModel.Price;
            existingProduct.CategoryId = productModel.CategoryId;
            existingProduct.DateUpdated = DateTime.UtcNow;

            // Заміна URL обкладинки, якщо було завантажено нову
            if (!string.IsNullOrEmpty(productModel.CoverImageUrl))
            {
                existingProduct.CoverImageUrl = productModel.CoverImageUrl;
            }

            // Оновлення галереї, тільки якщо були завантажені нові файли
            if (productModel.Gallery != null && productModel.Gallery.Any())
            {
                existingProduct.ProductImages.Clear();
                foreach (var gallery in productModel.Gallery)
                {
                    existingProduct.ProductImages.Add(new ProductImage
                    {
                        Name = gallery.Name,
                        ImageURL = gallery.ImageURL
                    });
                }
            }

            _context.Product.Update(existingProduct);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteProduct(Guid productId)
        {
            var product = await _context.Product.FindAsync(productId);
            if (product == null) return false;

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
