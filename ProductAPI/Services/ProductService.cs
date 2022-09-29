using Grpc.Net.Client;
using Nest;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Services.Interfaces;
using ProductGrpcService;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly DBContextClass _dbContext;
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductService> _logger;
        private readonly GrpcChannel _channel;
        private readonly ProductGrpcService.ProductGrpcServiceClient.ProductGrpcServiceClientClient _client;
        private readonly IConfiguration _configuration;

        public ProductService(DBContextClass dbContext, IElasticClient elasticClient, ILogger<ProductService> logger, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _elasticClient = elasticClient;
            _configuration = configuration;
            _channel = GrpcChannel.ForAddress(_configuration.GetValue<string>("GrpcSettings:OfferServiceUrl"));
            _client = new ProductGrpcService.ProductGrpcServiceClient.ProductGrpcServiceClientClient(_channel);


        }
        public IEnumerable<Models.Product> GetProductList()
        {
            return _dbContext.Products.ToList();
        }

        public async Task<ProductGrpcService.ProductGrpcs> GetProductListViaGrpc()
        {
            var result = _client.GetOfferList(new Empty { });
            return null;
        }

        public async Task<IEnumerable<Models.Product>> GetProductElastic(string keyword)
        {
            var result = await _elasticClient.SearchAsync<Models.Product>(
                             s => s.Query(
                                 q => q.QueryString(
                                     d => d.Query('*' + keyword + '*')
                                 )).Size(5000));

            _logger.LogInformation("ProductsController Get - ", DateTime.UtcNow);
            return result.Documents.ToList();
        }

        public Models.Product GetProductById(int id)
        {
            return _dbContext.Products.Where(x => x.ProductId == id).FirstOrDefault();
        }

        public async Task<Models.Product> AddProduct(Models.Product product)
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

        public Models.Product UpdateProduct(Models.Product product)
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
