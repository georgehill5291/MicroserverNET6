using Microsoft.AspNetCore.Mvc;
using ProductAPI.Cache;
using ProductAPI.Models;
using ProductAPI.Services.Interfaces;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICacheService _cacheService;
        public ProductController(IProductService productService, ICacheService cacheService)
        {
            this._productService = productService;
            _cacheService = cacheService;
        }
        [HttpGet]
        public IEnumerable<Product> ProductList()
        {
            var productList = _productService.GetProductList();
            return productList;
        }
        [HttpGet("Search/{keyword}")]
        public async Task<IEnumerable<Product>> ProductElasticSearch(string keyword = "")
        {
            var productList = await _productService.GetProductElastic(keyword);
            return productList;
        }
        [HttpGet("grpc")]
        public async Task<ProductGrpcService.ProductGrpcs> GetProductListViaGrpc()
        {
            var productList = await _productService.GetProductListViaGrpc();
            return productList;
        }
        [HttpGet("RedisCache")]
        public IEnumerable<Product> GetProductListViaRedisCache()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null)
            {
                return cacheData;
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = _productService.GetProductList();
            _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return cacheData;
        }
        [HttpGet("{id}")]
        public Product GetProductById(int id)
        {
            return _productService.GetProductById(id);
        }
        [HttpPost]
        public async Task<Product> AddProduct(Product product)
        {
            return await _productService.AddProduct(product);
        }
        [HttpPut]
        public Product UpdateProduct(Product product)
        {
            return _productService.UpdateProduct(product);
        }
        [HttpDelete("{id}")]
        public bool DeleteProduct(int id)
        {
            return _productService.DeleteProduct(id);
        }
    }
}
