using ProductAPI.Models;

namespace CategoryAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        public IEnumerable<Category> GetCategoryList();
        Task<IEnumerable<Category>> GetCategoryElastic(string keyword);
        public Task<Category> AddCategory(Category Category);
    }
}
