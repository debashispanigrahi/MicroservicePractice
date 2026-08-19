using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebSocketEmployeeApi.Models;
using WebSocketEmployeeApi.Services;

namespace WebSocketEmployeeApi.WebSockets;

public class EmployeeWebSocketHandler(
    IEmployeeService employeeService)
{
    public async Task HandleAsync(WebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var message = await ReceiveMessageAsync(webSocket);

                if (message == null)
                {
                    return;
                }

                await ProcessMessageAsync(
                    webSocket,
                    message);
            }
            catch (WebSocketException)
            {
                return;
            }
            catch (Exception ex)
            {
                await SendResponseAsync(
                    webSocket,
                    new WebSocketResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
        }
    }

    private static async Task<string?> ReceiveMessageAsync(
        WebSocket webSocket)
    {
        using var stream = new MemoryStream();

        var buffer = new byte[4096];

        while (true)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                if (webSocket.State == WebSocketState.Open ||
                    webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Connection closed by client.",
                        CancellationToken.None);
                }

                return null;
            }

            if (result.MessageType != WebSocketMessageType.Text)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.InvalidMessageType,
                    "Only text messages are supported.",
                    CancellationToken.None);

                return null;
            }

            await stream.WriteAsync(
                buffer.AsMemory(0, result.Count));

            if (result.EndOfMessage)
            {
                break;
            }
        }

        return Encoding.UTF8.GetString(
            stream.ToArray());
    }

    private async Task ProcessMessageAsync(
        WebSocket webSocket,
        string message)
    {
        try
        {
            var request = JsonSerializer.Deserialize<WebSocketRequest>(
                message,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (request == null)
            {
                await SendResponseAsync(
                    webSocket,
                    new WebSocketResponse
                    {
                        Success = false,
                        Message = "Invalid request."
                    });

                return;
            }

            switch (request.Action.ToUpperInvariant())
            {
                case "ADD_EMPLOYEE":

                    await AddEmployeeAsync(
                        webSocket,
                        request);

                    break;

                case "GET_ALL":

                    await GetAllEmployeesAsync(
                        webSocket);

                    break;

                case "BULK_ADD":

                    await AddEmployeesAsync(
                        webSocket,
                        request);

                    break;

                default:

                    await SendResponseAsync(
                        webSocket,
                        new WebSocketResponse
                        {
                            Success = false,
                            Message =
                                $"Unknown action: {request.Action}"
                        });

                    break;
            }
        }
        catch (JsonException)
        {
            await SendResponseAsync(
                webSocket,
                new WebSocketResponse
                {
                    Success = false,
                    Message = "Invalid JSON."
                });
        }
        catch (Exception ex)
        {
            await SendResponseAsync(
                webSocket,
                new WebSocketResponse
                {
                    Success = false,
                    Message = ex.Message
                });
        }
    }

    private async Task AddEmployeeAsync(
        WebSocket webSocket,
        WebSocketRequest request)
    {
        var employee =
            request.Data.Deserialize<CreateEmployeeRequest>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (employee == null)
        {
            await SendResponseAsync(
                webSocket,
                new WebSocketResponse
                {
                    Success = false,
                    Message = "Invalid employee data."
                });

            return;
        }

        var employeeId =
            await employeeService.AddEmployeeAsync(
                employee);

        await SendResponseAsync(
            webSocket,
            new WebSocketResponse
            {
                Success = true,
                Message = "Employee added successfully.",
                Data = new
                {
                    EmployeeId = employeeId
                }
            });
    }

    private async Task AddEmployeesAsync(
        WebSocket webSocket,
        WebSocketRequest request)
    {
        var employees =
            request.Data.Deserialize<List<CreateEmployeeRequest>>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (employees == null || employees.Count == 0)
        {
            await SendResponseAsync(
                webSocket,
                new WebSocketResponse
                {
                    Success = false,
                    Message = "No employees were provided."
                });

            return;
        }

        var count =
            await employeeService.AddEmployeesAsync(
                employees);

        await SendResponseAsync(
            webSocket,
            new WebSocketResponse
            {
                Success = true,
                Message = "Employees added successfully.",
                Data = new
                {
                    Count = count
                }
            });
    }

    private async Task GetAllEmployeesAsync(
        WebSocket webSocket)
    {
        var employees =
            await employeeService.GetAllEmployeesAsync();

        await SendResponseAsync(
            webSocket,
            new WebSocketResponse
            {
                Success = true,
                Message = "Employees retrieved successfully.",
                Data = employees
            });
    }

    private static async Task SendResponseAsync(
        WebSocket webSocket,
        WebSocketResponse response)
    {
        var json = JsonSerializer.Serialize(response);

        var bytes = Encoding.UTF8.GetBytes(json);

        await webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
}