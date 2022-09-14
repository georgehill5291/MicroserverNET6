using CategoryAPI.Services.Interfaces;
using Nest;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Services.Interfaces;

namespace ProductAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly DBContextClass _dbContext;
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(DBContextClass dbContext, IElasticClient elasticClient, ILogger<CategoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _elasticClient = elasticClient;
        }
        public IEnumerable<Category> GetCategoryList()
        {
            return _dbContext.Categories.ToList();
        }

        public async Task<IEnumerable<Category>> GetCategoryElastic(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Category>(
                             s => s.Query(
                                 q => q.QueryString(
                                     d => d.Query('*' + keyword + '*')
                                 )).Size(5000));

            //_logger.LogInformation("ProductsController Get - ", DateTime.UtcNow);
            return result.Documents.ToList();
        }

        public async Task<Category> AddCategory(Category category)
        {
            var result = _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            try
            {
                var response = await _elasticClient.IndexDocumentAsync(category);
            //    var response = _elasticClient.Search<Category>(s => s
            //.Index(Indices.Index(typeof(Category)).And(typeof(Category))));
            }
            catch (Exception ex)
            {

                throw;
            }

            return result.Entity;
        }

    }
}
