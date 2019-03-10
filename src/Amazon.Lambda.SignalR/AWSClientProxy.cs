using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.SignalR
{
    public class AWSClientProxy : IClientProxy
    {
        private readonly Func<IAWSSocketConnectionStore<SocketConnection>, Task<List<SocketConnection>>> _getConnections;
        private readonly IAWSSocketConnectionStore<SocketConnection> _connectionStore;
        private readonly IAWSSocketManager _awsSocketManager;
        private ILogger<AWSClientProxy> _logger;

        public AWSClientProxy(Func<IAWSSocketConnectionStore<SocketConnection>, Task<List<SocketConnection>>> getConnections,
            IAWSSocketConnectionStore<SocketConnection> connectionStore,
            IAWSSocketManager awsSocketManager
            )
        {
            this._getConnections = getConnections;
            this._connectionStore = connectionStore;
            this._awsSocketManager = awsSocketManager;
        }


        public async Task SendCoreAsync(string method, object[] args, CancellationToken cancellationToken = default(CancellationToken))
        {

            var connections = await _getConnections(_connectionStore);

            var sendToConnectionTasks = new List<Task>();
            foreach (var connection in connections)
            {
                sendToConnectionTasks.Add(_awsSocketManager.SendCoreAsync(connection.ConnectionId, method, args ?? null, cancellationToken));
            }

            Task.WaitAll(sendToConnectionTasks.ToArray());


            return;
        }
    }
}