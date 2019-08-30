using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreWebSocket.Common.WebSocketHelper
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _socketConn =
            new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(WebSocket socket)
        {
            var socketId = CreateConnectionId();
            while (!_socketConn.TryAdd(socketId, socket))
            {
                socketId = CreateConnectionId();
            }
        }

        public async Task RemoveSocket(string id)
        {
            try
            {
                _socketConn.TryRemove(id, out WebSocket socket);
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            catch (Exception ex)
            {
                //todo log
            }
        }

        public WebSocket GetSocketById(string id)
        {
            return _socketConn.FirstOrDefault(x => x.Key == id).Value;
        }

        public string GetSocketId(WebSocket socket)
        {
            return _socketConn.FirstOrDefault(x => x.Value == socket).Key;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _socketConn;
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}