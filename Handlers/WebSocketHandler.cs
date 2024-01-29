using System.Collections.Concurrent;
using System.Net.WebSockets;
using ECommerceApp.Database;

public class WebSocketHandler
{
    protected ApplicationDBContext _dbContext { get; set; }

    public WebSocketHandler(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public virtual async Task OnConnected(WebSocket socket)
    {
        var socketId = Guid.NewGuid().ToString();

        _sockets.TryAdd(socketId, socket);
    }

    public virtual async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        // Handle received message, e.g. save it to the database
    }

    public virtual async Task OnDisconnected(WebSocket socket)
    {
        // Handle disconnection, e.g. remove the socket from the list of connected sockets
    }
}