using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

public static class WebSocketHandler
{
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

    public static async Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string connectionId = Guid.NewGuid().ToString();
            _sockets[connectionId] = webSocket;

            await ReceiveMessagesAsync(webSocket, connectionId);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task ReceiveMessagesAsync(WebSocket webSocket, string connectionId)
    {
        byte[] buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // Broadcast received message to all connected clients (for demonstration)
            foreach (var socket in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        _sockets.TryRemove(connectionId, out _);
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}