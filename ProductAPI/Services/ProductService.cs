using Nest;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Services.Interfaces;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly DBContextClass _dbContext;
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(DBContextClass dbContext, IElasticClient elasticClient, ILogger<ProductService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _elasticClient = elasticClient;
        }
        public IEnumerable<Product> GetProductList()
        {
            return _dbContext.Products.ToList();
        }
        public async Task<IEnumerable<Product>> GetProductElastic(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Product>(
                             s => s.Query(
                                 q => q.QueryString(
                                     d => d.Query('*' + keyword + '*')
                                 )).Size(5000));

            _logger.LogInformation("ProductsController Get - ", DateTime.UtcNow);
            return result.Documents.ToList();
        }

        public Product GetProductById(int id)
        {
            return _dbContext.Products.Where(x => x.ProductId == id).FirstOrDefault();
        }

        public async Task<Product> AddProduct(Product product)
        {
            var result = _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            try
            {
                await _elasticClient.IndexDocumentAsync(product);
            }
            catch (Exception ex)
            {

                throw;
            }

            return result.Entity;
        }

        public Product UpdateProduct(Product product)
        {
            var result = _dbContext.Products.Update(product);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        public bool DeleteProduct(int Id)
        {
            var filteredData = _dbContext.Products.Where(x => x.ProductId == Id).FirstOrDefault();
            var result = _dbContext.Remove(filteredData);
            _dbContext.SaveChanges();
            return result != null ? true : false;
        }
    }
}
