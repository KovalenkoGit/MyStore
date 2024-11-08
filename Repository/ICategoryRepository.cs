using MyStore.Models;

namespace MyStore.Repository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryModels>> GetCategory();
    }
}