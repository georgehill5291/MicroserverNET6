using ProductGrpcService.Entities;

namespace ProductAPI.Services.Interfaces
{
    public interface IProductGrpcService
    {
        public List<ProductGrpc> GetOfferList();
    }
}
