using ProductAPI.Models;

namespace ProductAPI.Services.Interfaces
{
    public interface IProductService
    {
        public IEnumerable<Product> GetProductList();
        public Product GetProductById(int id);
        Task<IEnumerable<Product>> GetProductElastic(string keyword);
        Task<ProductGrpcService.ProductGrpcs> GetProductListViaGrpc();
        public Task<Product> AddProduct(Product product);
        public Product UpdateProduct(Product product);
        public bool DeleteProduct(int Id);
    }
}
