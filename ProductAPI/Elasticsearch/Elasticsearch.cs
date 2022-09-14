using Nest;
using ProductAPI.Models;

namespace ProductAPI.Elasticsearch
{
    public static class Elasticsearch
    {
        public static void AddElasticsearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ELKConfiguration:Uri"];
            var productIndex = configuration["ELKConfiguration:productIndex"];
            var categoryIndex = configuration["ELKConfiguration:categoryIndex"];

            var settings = new ConnectionSettings(new Uri(url)).BasicAuthentication(username: "", password: "")
                .PrettyJson()
                .DefaultIndex(productIndex);

            AddDefaultMappings(settings, configuration);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, productIndex);
            CreateIndex(client, categoryIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings, IConfiguration configuration)
        {
            var productIndex = configuration["ELKConfiguration:productIndex"];
            var categoryIndex = configuration["ELKConfiguration:categoryIndex"];

            settings
                .DefaultMappingFor<Product>(m => m.IndexName(productIndex).IdProperty(f => f.ProductId)
                //.Ignore(p => p.ProductName)
                //.Ignore(p => p.ProductPrice)
                )
                .DefaultMappingFor<Category>(i => i.IndexName(categoryIndex).IdProperty(f => f.CategoryId))
;
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices
                .Create(indexName,index => index.Map<Product>(x => x.AutoMap()));

            client.Indices
                .Create(indexName, index => index.Map<Category>(x => x.AutoMap()));
        }
    }
}
