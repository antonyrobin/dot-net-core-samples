using BlazorApp.Models;
using BlazorApp.Repositories.Interfaces;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace BlazorApp.Repositories.Implementations
{
    public class FormSubmissionCosmosRepository : IFormSubmissionRepository
    {
        private readonly Container _container;
        private readonly string _partitionKeyPath;
        private const string DatabaseName = "dynamicsdb";
        private const string ContainerName = "dynamics_submissions";

        public FormSubmissionCosmosRepository(CosmosClient cosmosClient)
        {
            _container = cosmosClient.GetContainer(DatabaseName, ContainerName);
            // Read the container properties at startup to determine the partition key path
            // Use a synchronous wait here because constructors cannot be async; this runs once at app startup.
            _partitionKeyPath = _container.ReadContainerAsync().GetAwaiter().GetResult().Resource.PartitionKeyPath;
        }

        public async Task<List<FormSubmission>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<FormSubmission>("SELECT * FROM c");
            var results = new List<FormSubmission>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<FormSubmission?> GetByIdAsync(string id)
        {
            // Read by id using a query so we don't need to assume the partition key value
            var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id").WithParameter("@id", id);
            var it = _container.GetItemQueryIterator<FormSubmission>(query, requestOptions: new QueryRequestOptions { MaxItemCount = 1 });
            while (it.HasMoreResults)
            {
                var resp = await it.ReadNextAsync();
                if (resp.Count > 0) return resp.First();
            }
            return null;
        }

        public async Task SaveAsync(FormSubmission submission)
        {
            var pkValue = GetPartitionKeyValue(submission);
            if (pkValue == null)
            {
                throw new InvalidOperationException($"Partition key path '{_partitionKeyPath}' not found in document. Ensure the document contains the partition key value.");
            }
            await _container.UpsertItemAsync(submission, new PartitionKey(pkValue));
        }

        public async Task UpdateAsync(FormSubmission submission)
        {
            var pkValue = GetPartitionKeyValue(submission);
            if (pkValue == null)
            {
                throw new InvalidOperationException($"Partition key path '{_partitionKeyPath}' not found in document. Ensure the document contains the partition key value.");
            }
            await _container.UpsertItemAsync(submission, new PartitionKey(pkValue));
        }

        public async Task DeleteAsync(string id)
        {
            // Locate the item (and its partition key) via a query, then delete with the correct PK
            var existing = await GetByIdAsync(id);
            if (existing == null) return;
            var pkValue = GetPartitionKeyValue(existing);
            if (pkValue == null)
            {
                throw new InvalidOperationException($"Partition key path '{_partitionKeyPath}' not found in document. Cannot delete item '{id}'.");
            }
            await _container.DeleteItemAsync<FormSubmission>(id, new PartitionKey(pkValue));
        }

        private string? GetPartitionKeyValue(object item)
        {
            if (string.IsNullOrWhiteSpace(_partitionKeyPath) || _partitionKeyPath == "/") return null;

            // Serialize the object using System.Text.Json so any JsonPropertyName attributes are respected
            var json = JsonSerializer.Serialize(item);
            using var doc = JsonDocument.Parse(json);
            var segments = _partitionKeyPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            JsonElement element = doc.RootElement;
            foreach (var seg in segments)
            {
                if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(seg, out element))
                {
                    return null;
                }
            }

            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => null,
                _ => element.GetRawText(),
            };
        }
    }
}
