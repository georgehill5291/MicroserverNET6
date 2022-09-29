using Microsoft.EntityFrameworkCore;
using ProductAPI.Services.Interfaces;
using ProductGrpcService.Data;
using ProductGrpcService.Entities;

namespace ProductGrpcService.Services
{
    public class ProductGrpcService : IProductGrpcService
    {
        private readonly DbContextClass _dbContext;
        private readonly ILogger<ProductGrpcService> _logger;

        public ProductGrpcService(DbContextClass dbContext, ILogger<ProductGrpcService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<ProductGrpc> GetOfferList()
        {
            return _dbContext.Product.ToList();
        }

    }
}
