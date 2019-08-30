using Microsoft.AspNetCore.Http;

namespace AspNetCoreWebSocket.Common.HttpContextHelper
{
    public class WsHttpContext
    {
        private static IHttpContextAccessor httpContextAccessor;
        public static HttpContext Current => httpContextAccessor.HttpContext;
        public static string AppBaseUrl => $"{Current.Request.Host}";
        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            httpContextAccessor = contextAccessor;
        }
    }
}