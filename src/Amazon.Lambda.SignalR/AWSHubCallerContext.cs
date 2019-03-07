using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace Amazon.Lambda.SignalR
{
    public class AWSHubCallerContext : HubCallerContext
    {
        private readonly HttpContext _httpContext;
        private readonly IAWSWebSocketFeature _webSocketFeaure;

        public AWSHubCallerContext(HttpContext httpContext)
        {
            this._httpContext = httpContext;

            _webSocketFeaure = _httpContext.Features.Get<IAWSWebSocketFeature>();
        }

        public override string ConnectionId => _webSocketFeaure.ConnectionId;

        public override string UserIdentifier => _httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public override ClaimsPrincipal User => _httpContext.User;

        public override IDictionary<object, object> Items => _httpContext.Items;

        public override IFeatureCollection Features => _httpContext.Features;

        public override CancellationToken ConnectionAborted => _httpContext.RequestAborted;

        public override void Abort() => _httpContext.Abort();

    }
}