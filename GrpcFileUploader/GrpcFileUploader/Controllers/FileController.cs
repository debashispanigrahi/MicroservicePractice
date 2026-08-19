using Google.Protobuf;
using Grpc.Net.Client;
using GrpcFileService;
using GrpcFileUploader.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrpcFileUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController(IEmployeeWebSocketClient webSocketClient) : ControllerBase
    {
        [HttpPost("grpc/upload")]
        public async Task<IActionResult> UploadFileForGrpc(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a file.");
            }

            // Read IFormFile into memory
            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            byte[] fileBytes = memoryStream.ToArray();

            // Create gRPC channel
            using var channel = GrpcChannel.ForAddress(
                "https://localhost:7266");

            // Create generated gRPC client
            var client = new FileService.FileServiceClient(channel);

            // Call gRPC
            var response = await client.UploadFileAsync(
                new UploadFileRequest
                {
                    FileName = file.FileName,
                    FileContent = ByteString.CopyFrom(fileBytes)
                });

            return Ok(new
            {
                response.Success,
                response.Message
            });
        }

        [HttpPost("ws/upload")]
        public async Task<IActionResult> UploadFileForWS(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please provide a file.");
            }

            if (!Path.GetExtension(file.FileName)
                .Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .txt files are supported.");
            }

            using var reader = new StreamReader(file.OpenReadStream());

            var content = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("File is empty.");
            }

            var response =
                await webSocketClient.SendEmployeesAsync(content);

            return Ok(response);
        }
    }
}
