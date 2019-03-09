using System.IO;
using System.Threading.Tasks;
using Leelou.Lea.Api.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace serverless.Hubs
{


    public class TestHub : AWSSockerServiceHub
    {
        private readonly ILogger<TestHub> _logger;

        public TestHub(
            HubCallerContext hubCallerContext,
            IGroupManager groups,
            IHubClients clients,
            ILogger<TestHub> logger
         ) : base(hubCallerContext, groups, clients)
        {
            this._logger = logger;
        }


        public override Task OnConnectedAsync()
        {


            _logger.LogError("Yay connected");


            return Task.CompletedTask;
        }



        public override async Task OnMessageReceivedAsync(Stream content)
        {
            string contentString;
            using (var ms = new StreamReader(content))
            {
                contentString = await ms.ReadToEndAsync();
            }

            if (content == null) return;


            _logger.LogError("Socket: message"+content);


            return;
        }


        public override Task OnDisconnectedAsync(System.Exception exception)
        {


            return Task.CompletedTask;
        }
    }
}