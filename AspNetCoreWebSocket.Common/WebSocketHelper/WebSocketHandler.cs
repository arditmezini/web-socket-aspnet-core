using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreWebSocket.Common.WebSocketHelper
{
    public abstract class WebSocketHandler
    {
        public WebSocketConnectionManager _webSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
        {
            _webSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            _webSocketConnectionManager.AddSocket(socket);

            await SendMessageAsync(socket, new JObject { { "Message", "Accepted" } });
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var socketId = _webSocketConnectionManager.GetSocketId(socket);
            await _webSocketConnectionManager.RemoveSocket(socketId);
        }

        public async Task SendMessageAsync(string socketId, JObject message)
        {
            await SendMessageAsync(_webSocketConnectionManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageToAllAsync(JObject message)
        {
            foreach (var pair in _webSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);

        private async Task SendMessageAsync(WebSocket socket, JObject message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(
                    buffer: new ArraySegment<byte>(
                        array: Encoding.ASCII.GetBytes(message.ToString()),
                        offset: 0,
                        count: message.ToString().Length),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None
                );
        }
    }
}