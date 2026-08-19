using System.Net.WebSockets;
using System.Text;

namespace GrpcFileUploader.Services
{
    public interface IEmployeeWebSocketClient
    {
        Task<string> SendEmployeesAsync(string content);
    }

    public class EmployeeWebSocketClient : IEmployeeWebSocketClient
    {
        private readonly IConfiguration _configuration;

        public EmployeeWebSocketClient(IConfiguration configuration) => _configuration = configuration;

        public async Task<string> SendEmployeesAsync(string content)
        {
            var webSocketUrl =
                _configuration["EmployeeWebSocket:Url"];

            using var webSocket = new ClientWebSocket();

            await webSocket.ConnectAsync(
                new Uri(webSocketUrl!),
                CancellationToken.None);

            var message = $$"""
                            {
                                "action": "BULK_ADD",
                                "data": {{content}}
                            }
                            """;

            var bytes = Encoding.UTF8.GetBytes(message);

            await webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            return await ReceiveResponseAsync(webSocket);
        }

        private static async Task<string> ReceiveResponseAsync(
    ClientWebSocket webSocket)
        {
            using var stream = new MemoryStream();

            var buffer = new byte[4096];

            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new WebSocketException(
                        "WebSocket server closed the connection.");
                }

                await stream.WriteAsync(
                    buffer.AsMemory(0, result.Count));

            } while (!result.EndOfMessage);

            return Encoding.UTF8.GetString(
                stream.ToArray());
        }
    }
}
