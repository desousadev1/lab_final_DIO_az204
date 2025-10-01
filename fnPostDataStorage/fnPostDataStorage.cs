using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace flixdio.Function;

public class fnPostDataStorage
{
    private readonly ILogger<fnPostDataStorage> _logger;

    public fnPostDataStorage(ILogger<fnPostDataStorage> logger)
    {
        _logger = logger;
    }

    [Function("dataStorage")]//fnPostDataStorage
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("Processando arquivos no storage.");
        try
        {
            if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
            {
                return new BadRequestObjectResult("O cabeçalho 'file-type' está ausente.");
            }
            var fileType = fileTypeHeader.ToString();
            var form = await req.ReadFormAsync();
            var file = form.Files["file"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("O arquivo não foi enviado.");
            }

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var containerName = fileType;
            BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            string blobName = file.FileName;
            var blob = containerClient.GetBlobClient(blobName);
            

            using (var stream = file.OpenReadStream())
            {
                await blob.UploadAsync(stream, overwrite: true);
            }            

            string mensagemSucesso = $"Arquivo \"{file.FileName}\" armazenado com sucesso.";
            _logger.LogInformation(mensagemSucesso);
            return new OkObjectResult(new
            {
                Message = mensagemSucesso,
                BlobUrl = blob.Uri.ToString()
            });
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao processar arquivos: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }  
    }
}