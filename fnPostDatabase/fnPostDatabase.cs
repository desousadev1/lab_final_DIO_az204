using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace fnPostDatabase;

public class fnPostDatabase
{
    private readonly ILogger<fnPostDatabase> _logger;

    public fnPostDatabase(ILogger<fnPostDatabase> logger)
    {
        _logger = logger;
    }

    [Function("movie")]
    [CosmosDBOutput(
        "%CosmosDBDatabaseName%",
        "%CosmosDBContainerName%",
        Connection = "CosmosDBConnectionString",
        CreateIfNotExists = true,
        PartitionKey = "/id")]
    public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        MovieRequest? movie = null;

        try
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                movie = JsonConvert.DeserializeObject<MovieRequest?>(body);
            }
            return JsonConvert.SerializeObject(movie);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error parsing request body: {ex.Message}");
            return new BadRequestObjectResult("Invalid request body");
        }        
    }        
    
}
