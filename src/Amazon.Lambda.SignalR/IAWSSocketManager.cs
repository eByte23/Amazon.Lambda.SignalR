using System;
using System.IO;
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
            _logger.LogInformation($"Sending data to connectionId: {connectionId} data: {method}");
            try
            {
                PostToConnectionResponse sendResult;
                using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes((string)method)))
                {
                    sendResult = await _apiGatewayManagementApi.PostToConnectionAsync(new PostToConnectionRequest()
                    {
                        ConnectionId = connectionId,
                        Data = ms
                    });
                }
                if (sendResult.HttpStatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else if (sendResult.HttpStatusCode == HttpStatusCode.Gone)
                {
                    Task.Run(() => _connectionStore.RemoveConnectionAsync(connectionId));
                }
                else
                {
                    _logger.LogError($"Error1 sending message to connectionId: {connectionId}, httpStatus: {sendResult.HttpStatusCode}");
                }
            }
            catch (GoneException e)
            {
                _logger.LogWarning($"ConnectionId: {connectionId} received a GoneException, {e.Message.ToString()} ,{e.InnerException?.Message?.ToString()}");
                await _connectionStore.RemoveConnectionAsync(connectionId);
            }
            catch (Amazon.ApiGatewayManagementApi.Model.ForbiddenException e)
            {
                _logger.LogError($"Error2 sending message to connectionId: {connectionId}, {e.Message.ToString()} ,{e.InnerException?.Message?.ToString()}");
            }
            catch (Amazon.ApiGatewayManagementApi.Model.PayloadTooLargeException e)
            {
                _logger.LogError($"Error3 sending message to connectionId: {connectionId}, {e.Message.ToString()} ,{e.InnerException?.Message?.ToString()}");
            }
            catch (AggregateException e)
            {
                _logger.LogError($"Error4 sending message to connectionId: {connectionId}, {e.Message.ToString()} ,{e.InnerException?.Message?.ToString()}");
            }
            catch (ObjectDisposedException e)
            {
                _logger.LogWarning($"Removing connectionId: {connectionId} from store.");
                await _connectionStore.RemoveConnectionAsync(connectionId);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error5 sending message to connectionId: {connectionId}, {e.Message.ToString()} ,{e.InnerException?.Message?.ToString()}");
                _logger.LogWarning($"Removing connectionId: {connectionId} from store.");
                await _connectionStore.RemoveConnectionAsync(connectionId);
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