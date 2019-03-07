using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Amazon.Lambda.SignalR
{
    public class AWSSocketGroupManager : IGroupManager
    {
        private readonly IAWSSocketConnectionStore<SocketConnection> _connectionStore;

        public AWSSocketGroupManager(IAWSSocketConnectionStore<SocketConnection> connectionStore)
        {
            this._connectionStore = connectionStore;
        }

        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connectionId == null) throw new ArgumentNullException(connectionId);

            if (groupName == null) throw new ArgumentNullException(groupName);

            return _connectionStore.AddConnectionToGroupAsync(connectionId, groupName);
        }

        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connectionId == null) throw new ArgumentNullException(connectionId);

            if (groupName == null) throw new ArgumentNullException(groupName);

            return _connectionStore.AddConnectionToGroupAsync(connectionId, groupName);
        }
    }
}