using AspNetCoreWebSocket.Api.WebSocketHandlers;
using AspNetCoreWebSocket.Common.WebSocketHelper;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreWebSocket.Api.Configurations
{
    public static class StartupConfigurations
    {
        public static void ConfigureWebSockets(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseWebSockets();

            var wsOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            // Whitelist the allowed websocket connection ips
            //Comment out  the line below if you need to whitelist ip
            //wsOptions.AllowedOrigins.Add(“ip”);

            app.UseWebSockets(wsOptions);
            app.MapWebSocket("/student", serviceProvider.GetService<StudentWebsocketHandler>());
            app.MapWebSocket("/course", serviceProvider.GetService<CourseWebSocketHandler>());
        }
    }
}