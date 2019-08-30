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
    public class CourseWebSocketHandler : WebSocketHandler
    {
        private readonly IServiceScopeFactory _service;

        public CourseWebSocketHandler(WebSocketConnectionManager webSocketConnectionManager, IServiceScopeFactory service)
            : base(webSocketConnectionManager)
        {
            _service = service;
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var courseObj = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Course course = JsonConvert.DeserializeObject<Course>(courseObj);

            var responseObject = await GetResponseObjectAsync(course);

            await SendMessageToAllAsync(responseObject);
        }

        private async Task<JObject> GetResponseObjectAsync(Course course)
        {
            using (var scope = _service.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<SocketContext>();
                if (course.Id == 0)
                {
                    await context.Courses.AddAsync(course);
                    await context.SaveChangesAsync();
                }
                else
                {
                    var entity = await context.Courses.FirstOrDefaultAsync(x => x.Id == course.Id);
                    if (entity != null)
                    {
                        entity.Name = course.Name;
                        entity.CourseCode = course.CourseCode;

                        await context.SaveChangesAsync();
                    }
                }

                var reloadEntity = await context.Courses.SingleOrDefaultAsync(x => x.Id == course.Id);
                var jsonEntity = JsonConvert.SerializeObject(reloadEntity);
                JObject jEntity = JObject.Parse(jsonEntity);
                return JObject.FromObject(jEntity);
            }
        }
    }
}