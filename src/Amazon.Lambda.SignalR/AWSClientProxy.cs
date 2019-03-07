using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Amazon.Lambda.SignalR
{
    public class AWSClientProxy : IClientProxy
    {
        private readonly Func<IAWSSocketConnectionStore<SocketConnection>, Task<List<SocketConnection>>> _getConnections;
        private readonly IAWSSocketConnectionStore<SocketConnection> _connectionStore;
        private readonly IAWSSocketManager _awsSocketManager;

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
            Task[] sendToConnectionTasks = new Task[] { };
            foreach (var connection in connections)
            {
                sendToConnectionTasks[sendToConnectionTasks.Length] = _awsSocketManager.SendCoreAsync(connection.ConnectionId, method, args, cancellationToken);
            }

            Task.WaitAll(sendToConnectionTasks);


            return;
        }
    }
}