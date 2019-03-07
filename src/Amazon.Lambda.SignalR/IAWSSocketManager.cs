using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.SignalR
{
    public class AWSSocketManager : IAWSSocketManager
    {
        private readonly IAmazonApiGatewayManagementApi _apiGatewayManagementApi;
        private readonly IAWSSocketConnectionStore<SocketConnection> _connectionStore;
        private readonly ILogger<AWSSocketManager> _logger;

        public AWSSocketManager(IAmazonApiGatewayManagementApi apiGatewayManagementApi, IAWSSocketConnectionStore<SocketConnection> connectionStore, ILogger<AWSSocketManager> logger)
        {
            this._apiGatewayManagementApi = apiGatewayManagementApi;
            this._connectionStore = connectionStore;
            this._logger = logger;
        }

        public async Task SendCoreAsync(string connectionId, string method, object[] args, CancellationToken cancellationToken)
        {
            try
            {

                var sendResult = await _apiGatewayManagementApi.PostToConnectionAsync(new PostToConnectionRequest()
                {
                    ConnectionId = connectionId
                });

                if (sendResult.HttpStatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else if (sendResult.HttpStatusCode == HttpStatusCode.Gone)
                {
                    Task.Run(() => _connectionStore.RemoveConnection(connectionId));
                }
                else
                {
                    _logger.LogError($"Error sending message to connectionId: {connectionId}, httpStatus: {sendResult.HttpStatusCode}");
                }
            }
            catch (GoneException e)
            {
                _logger.LogWarning($"ConnectionId: {connectionId} received a GoneException, {e.Message.ToString()}");
                await _connectionStore.RemoveConnection(connectionId);
            }
            catch (Amazon.ApiGatewayManagementApi.Model.ForbiddenException e)
            {
                _logger.LogError($"Error sending message to connectionId: {connectionId}, {e.Message.ToString()}");
            }
            catch (Amazon.ApiGatewayManagementApi.Model.PayloadTooLargeException e)
            {
                _logger.LogError($"Error sending message to connectionId: {connectionId}, {e.Message.ToString()}");
            }





            return;
        }

        public Task SendCoreAsync(string connectionId, string method, object[] args) => SendCoreAsync(connectionId, method, args, default(CancellationToken));

        public Task SendData(string connectionId, string data)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IAWSSocketManager
    {
        // Basic AWS Send Data
        Task SendData(string connectionIds, string data);


        // For SignalR Implementation Later
        Task SendCoreAsync(string connectionIds, string method, object[] args);
        Task SendCoreAsync(string connectionIds, string method, object[] args, CancellationToken cancellationToken);
    }
}