using System.Text.Json;
using Grpc.Core;
using GrpcFileService.Models;
using Google.Protobuf;
using GrpcFileService.Repo;

namespace GrpcFileService.Services;

public class FileService(IProductRepo productRepo) : GrpcFileService.FileService.FileServiceBase
{
    public override async Task<UploadFileResponse> UploadFile(
        UploadFileRequest request,
        ServerCallContext context)
    {
        try
        {
            // 1. Get the file bytes
            byte[] fileBytes = request.FileContent.ToByteArray();

            // 2. Convert bytes to string
            string fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);

            // 3. Deserialize JSON
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };

            List<Product>? products =
                JsonSerializer.Deserialize<List<Product>>(
                    fileContent,
                    options);

            if (products == null)
            {
                return new UploadFileResponse
                {
                    Success = false,
                    Message = "Unable to deserialize file content."
                };
            }

            await productRepo.InsertProductsAsync(products);

            return new UploadFileResponse
            {
                Success = true,
                Message = $"Successfully processed {products.Count} products."
            };
        }
        catch (Exception ex)
        {
            return new UploadFileResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}