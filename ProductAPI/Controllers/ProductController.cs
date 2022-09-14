using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Services.Interfaces;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        public ProductController(IProductService _productService)
        {
            productService = _productService;
        }
        [HttpGet]
        public IEnumerable<Product> ProductList()
        {
            var productList = productService.GetProductList();
            return productList;
        }
        [HttpGet("Search/{keyword}")]
        public async Task<IEnumerable<Product>> ProductElasticSearch(string keyword = "")
        {
            var productList = await productService.GetProductElastic(keyword);
            return productList;
        }
        [HttpGet("{id}")]
        public Product GetProductById(int id)
        {
            return productService.GetProductById(id);
        }
        [HttpPost]
        public async Task<Product> AddProduct(Product product)
        {
            return await productService.AddProduct(product);
        }
        [HttpPut]
        public Product UpdateProduct(Product product)
        {
            return productService.UpdateProduct(product);
        }
        [HttpDelete("{id}")]
        public bool DeleteProduct(int id)
        {
            return productService.DeleteProduct(id);
        }
    }
}
