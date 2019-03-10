using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.SignalR;
using Leelou.Lea.Api.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace serverless.Hubs
{


    public class TestHub : AWSSockerServiceHub
    {
        private readonly ILogger<TestHub> _logger;
        private readonly IAWSSocketConnectionStore<SocketConnection> _connectionStore;

        public TestHub(
            HubCallerContext hubCallerContext,
            IGroupManager groups,
            IHubClients clients,
            ILogger<TestHub> logger,
            IAWSSocketConnectionStore<SocketConnection> connectionStore
         ) : base(hubCallerContext, groups, clients)
        {
            this._logger = logger;
            this._connectionStore = connectionStore;
        }


        public override async Task OnConnectedAsync()
        {
            //Temp
            string userId = Guid.NewGuid().ToString();
            await _connectionStore.StoreConnectionAsync(Context.ConnectionId, userId);

            _logger.LogInformation($"Yay connected. ConnectionId: {Context.ConnectionId} is associated to userId: {userId}");

            return;
        }


        public static Task SendValueAsync(string value, IHubClients context)
        {


            return context.All.SendAsync(value,"sss");
        }

        public override async Task OnMessageReceivedAsync(Stream content)
        {
            string contentString;
            using (var ms = new StreamReader(content))
            {
                contentString = await ms.ReadToEndAsync();
            }

            if (content == null) return;


            _logger.LogInformation("Socket: message" + contentString);

            await Clients.All.SendAsync(contentString, "sss");


            return;
        }


        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            _logger.LogWarning($"Disconnection connectionId: {Context.ConnectionId}");

            await _connectionStore.RemoveConnectionAsync(Context.ConnectionId);
            return;
        }
    }
}