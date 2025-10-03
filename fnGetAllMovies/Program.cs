using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

/* builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights(); */
    
builder.Services.AddSingleton(s =>
{
    var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
    return new CosmosClient(cosmosDbConnectionString);
});

builder.Build().Run();
