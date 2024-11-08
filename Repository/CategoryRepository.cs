using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context = null;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryModels>> GetCategory()
        {
            return await _context.Category.Select(x => new CategoryModels()
            {
                CategoryId = x.CategoryId,
                Description = x.Description,
                Name = x.Name
            }).ToListAsync();
        }       
    }
}
