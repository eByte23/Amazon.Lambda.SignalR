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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAWSWebSocketFeature _webSocketFeaure;

        public AWSHubCallerContext(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;

            _webSocketFeaure = _httpContextAccessor.HttpContext.Features.Get<IAWSWebSocketFeature>();
        }

        public override string ConnectionId => _webSocketFeaure.ConnectionId;

        public override string UserIdentifier => _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public override ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;

        public override IDictionary<object, object> Items => _httpContextAccessor.HttpContext.Items;

        public override IFeatureCollection Features => _httpContextAccessor.HttpContext.Features;

        public override CancellationToken ConnectionAborted => _httpContextAccessor.HttpContext.RequestAborted;

        public override void Abort() => _httpContextAccessor.HttpContext.Abort();

    }
}