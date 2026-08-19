using System.Text.Json;

namespace WebSocketEmployeeApi.Models
{
    public class WebSocketRequest
    {
        public string Action { get; set; } = string.Empty;

        public JsonElement Data { get; set; }
    }
}
