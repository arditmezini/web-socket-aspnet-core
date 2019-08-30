using AspNetCoreWebSocket.Common.DataContext;
using AspNetCoreWebSocket.Common.Models;
using AspNetCoreWebSocket.Common.WebSocketHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreWebSocket.Api.WebSocketHandlers
{
    public class StudentWebsocketHandler : WebSocketHandler
    {
        private readonly IServiceScopeFactory _service;

        public StudentWebsocketHandler(WebSocketConnectionManager webSocketConnectionManager, IServiceScopeFactory service)
            : base(webSocketConnectionManager)
        {
            _service = service;
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var studentObj = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Student student = JsonConvert.DeserializeObject<Student>(studentObj);

            var responseObject = await GetResponseObjectAsync(student);

            await SendMessageToAllAsync(responseObject);
        }

        private async Task<JObject> GetResponseObjectAsync(Student student)
        {
            using (var scope = _service.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<SocketContext>();
                if (student.Id == 0)
                {
                    await context.Students.AddAsync(student);
                }
                else
                {
                    var entity = await context.Students.SingleOrDefaultAsync(x => x.Id == student.Id);
                    if (entity != null)
                    {
                        entity.Name = student.Name;
                        entity.Age = student.Age;

                        await context.SaveChangesAsync();
                    }
                }

                var reloadEntity = await context.Courses.SingleOrDefaultAsync(x => x.Id == student.Id);
                var jsonEntity = JsonConvert.SerializeObject(reloadEntity);
                JObject jEntity = JObject.Parse(jsonEntity);
                return JObject.FromObject(jEntity);
            }
        }
    }
}