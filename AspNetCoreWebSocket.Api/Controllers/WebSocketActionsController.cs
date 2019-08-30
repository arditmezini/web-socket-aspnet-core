using System.Collections.Generic;
using AspNetCoreWebSocket.Common.HttpContextHelper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebSocket.Api.Controllers
{
    [Route("api/ws-actions")]
    [ApiController]
    public class WebSocketActionsController : ControllerBase
    {
        // GET api/ws-actions
        [HttpGet]
        public ActionResult<IEnumerable<WebSocketActions>> Get()
        {
            return new List<WebSocketActions>
            {
                new WebSocketActions
                {
                    Description = "Student",
                    Url =$"wss://{WsHttpContext.AppBaseUrl}/student"
                },
                new WebSocketActions
                {
                    Description = "Course",
                    Url =$"wss://{WsHttpContext.AppBaseUrl}/course"
                }
            };
        }

        public class WebSocketActions
        {
            public string Description { get; set; }
            public string Url { get; set; }
        }
    }
}