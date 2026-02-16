using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using tasks.Models;

namespace tasks.Data
{
    public class CosmosService
    {
        private readonly Container _container;

        public CosmosService(IConfiguration configuration)
        {
            var account = configuration["CosmosDb:Account"];
            var key = configuration["CosmosDb:Key"];
            var databaseName = configuration["CosmosDb:DatabaseName"];
            var containerName = configuration["CosmosDb:ContainerName"];

            Console.WriteLine("===== COSMOS CONFIG =====");
            Console.WriteLine("ACCOUNT: " + account);
            Console.WriteLine("DATABASE: " + databaseName);
            Console.WriteLine("CONTAINER: " + containerName);
            Console.WriteLine("=========================");

            var client = new CosmosClient(account, key);

            var database = client.GetDatabase(databaseName);
            _container = database.GetContainer(containerName);
        }

        public async Task<List<CosmosMessage>> GetByGroupIdsAsync(List<Guid> groupIds)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE ARRAY_CONTAINS(@ids, c.groupId)")
                .WithParameter("@ids", groupIds);

            var result = new List<CosmosMessage>();
            var iterator = _container.GetItemQueryIterator<CosmosMessage>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result.AddRange(response);
            }

            return result;
        }

        public async Task<List<Guid>> GetAllGroupIdsAsync()
        {
            var query = new QueryDefinition(
                "SELECT DISTINCT VALUE c.groupId FROM c");

            var result = new List<Guid>();

            var iterator = _container.GetItemQueryIterator<string>(query);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    result.Add(Guid.Parse(item));
                }
            }

            return result;
        }




    }
}
