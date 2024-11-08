using MyStore.Models;
using System;

namespace MyStore.Repository
{
    public interface IProductRepository
    {
        Task<Guid> AddNewProduct(ProductModel product);
        Task<List<ProductModel>> GetAllProduct();
        Task<ProductModel> GetProductById(Guid productId);
        Task<List<ProductModel>> GetProductByCategoryId(Guid categoryId);
        Task<List<ProductModel>> GetTopProduct(int count);
        Task<List<ProductModel>> SearchProducts(string name);
        Task<List<GalleryModel>> GetExistingGallery(Guid productId);
        Task<bool> UpdateProduct(ProductModel productModel);
        Task<bool> DeleteProduct(Guid productId);
    }
}