using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace Amazon.Lambda.SignalR
{
    public class AWSHubCallerClients : IHubClients
    {
        private readonly IAWSSocketConnectionStore<SocketConnection> _socketConnectionStore;
        private readonly IAWSSocketManager _awsSocketManager;

        public AWSHubCallerClients(IAWSSocketConnectionStore<SocketConnection> socketConnectionStore, IAWSSocketManager awsSocketManager)
        {
            this._socketConnectionStore = socketConnectionStore;
            this._awsSocketManager = awsSocketManager;
        }

        public IClientProxy Caller => throw new NotImplementedException();

        public IClientProxy Others => throw new NotImplementedException();

        public IClientProxy All => throw new NotImplementedException();

        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds)
        {
            throw new NotImplementedException();
        }

        public IClientProxy Client(string connectionId)
        {
            return new AWSClientProxy(((x) => x.GetClient(connectionId)), _socketConnectionStore, _awsSocketManager);
        }

        public IClientProxy Clients(IReadOnlyList<string> connectionIds)
        {
            return new AWSClientProxy(((x) => x.GetClients(connectionIds)), _socketConnectionStore, _awsSocketManager);
        }

        public IClientProxy Group(string groupName)
        {
            return new AWSClientProxy(((x) => x.GetGroup(groupName)), _socketConnectionStore, _awsSocketManager);
        }

        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds)
        {
            throw new NotImplementedException();
        }

        public IClientProxy Groups(IReadOnlyList<string> groupNames)
        {
            throw new NotImplementedException();
        }

        public IClientProxy OthersInGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public IClientProxy User(string userId)
        {
            throw new NotImplementedException();
        }

        public IClientProxy Users(IReadOnlyList<string> userIds)
        {
            throw new NotImplementedException();
        }
    }
}