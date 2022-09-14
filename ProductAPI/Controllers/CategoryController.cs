using CategoryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Services.Interfaces;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        [HttpGet]
        public IEnumerable<Category> CategoryList()
        {
            var productList = categoryService.GetCategoryList();
            return productList;
        }

        [HttpGet("Search")]
        public async Task<IEnumerable<Category>> CategoryElasticSearch()
        {
            var categoryList = await categoryService.GetCategoryElastic("");
            return categoryList;
        }      

        [HttpPost]
        public async Task<Category> AddCategory(Category product)
        {
            return await categoryService.AddCategory(product);
        }
       
    }
}
