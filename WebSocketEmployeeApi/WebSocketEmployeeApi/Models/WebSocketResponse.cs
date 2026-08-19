namespace WebSocketEmployeeApi.Models
{
    public class WebSocketResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public object? Data { get; set; }
    }
}
