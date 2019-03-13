using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.SignalR
{
    public class DynamoDbSocketConnectionStore : IAWSSocketConnectionStore<SocketConnection>
    {
        private readonly IDynamoDBContext _context;
        private readonly ILogger<DynamoDbSocketConnectionStore> _logger;

        public DynamoDbSocketConnectionStore(IDynamoDBContext context, ILogger<DynamoDbSocketConnectionStore> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task AddConnectionToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = (await _context.QueryAsync<SocketConnection>(connectionId).GetRemainingAsync()).SingleOrDefault();

            if (connection == null) throw new NullReferenceException($"Could not add connectionId: {connectionId} to group: {groupName}. Socket Connection not found");

            if (connection.GroupIds == null)
            {
                connection.GroupIds = new List<string>();
            }

            if (!connection.GroupIds.Contains(groupName))
            {
                connection.GroupIds.Add(groupName);
                await _context.SaveAsync(connection, cancellationToken);
            }

            return;
        }

        public Task<List<SocketConnection>> GetAllClients()
        {
            return _context.ScanAsync<SocketConnection>(new List<ScanCondition>()).GetRemainingAsync();
        }

        public Task<List<SocketConnection>> GetAllClientsExcept(IReadOnlyList<string> connectionIds)
        {
            List<ScanCondition> conditions = new List<ScanCondition>()
            {
                new ScanCondition(nameof(SocketConnection.ConnectionId),ScanOperator.NotContains,connectionIds)
            };

            return _context.ScanAsync<SocketConnection>(conditions).GetRemainingAsync();
        }

        public Task<List<SocketConnection>> GetClient(string connectionId)
        {
            return _context.QueryAsync<SocketConnection>(connectionId).GetRemainingAsync();
        }

        public Task<List<SocketConnection>> GetClients(IReadOnlyList<string> connectionIds)
        {
            List<ScanCondition> conditions = new List<ScanCondition>()
            {
                new ScanCondition(nameof(SocketConnection.ConnectionId),ScanOperator.Contains,connectionIds)
            };

            return _context.ScanAsync<SocketConnection>(conditions).GetRemainingAsync();
        }

        public Task<List<SocketConnection>> GetGroup(string groupName)
        {
            List<ScanCondition> conditions = new List<ScanCondition>()
            {
                new ScanCondition(nameof(SocketConnection.GroupIds),ScanOperator.Contains,groupName)
            };

            return _context.ScanAsync<SocketConnection>(conditions).GetRemainingAsync();
        }

        public Task<List<SocketConnection>> GetUser(string userId)
        {

            // var search = _context.FromQueryAsync<SocketConnection>(new QueryOperationConfig
            // {
            //     KeyExpression = new Expression
            //     {
            //         ExpressionStatement = $"{nameof(SocketConnection.UserId)} = :v_Id",
            //         ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
            //         {
            //             {":v_Id", userId}
            //         }
            //     },
            //     Limit = 1
            // });
            List<ScanCondition> conditions = new List<ScanCondition>()
            {
                new ScanCondition(nameof(SocketConnection.UserId),ScanOperator.Equal,userId)
            };

            return _context.ScanAsync<SocketConnection>(conditions).GetRemainingAsync();

            // return search.GetRemainingAsync();
        }

        public Task<List<SocketConnection>> GetUsers(IReadOnlyList<string> userIds)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveConnectionFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken))
        {
            // throw new NotImplementedException();
            var connection = (await _context.QueryAsync<SocketConnection>(connectionId).GetRemainingAsync()).SingleOrDefault();

            if (connection == null) throw new NullReferenceException($"Could not remove connectionId: {connectionId} from group: {groupName}. Socket Connection not found");

            if (connection.GroupIds == null)
            {
                connection.GroupIds = new List<string>();
            }

            if (connection.GroupIds.Contains(groupName))
            {
                connection.GroupIds.Remove(groupName);
                await _context.SaveAsync(connection, cancellationToken);
            }

            return;
        }

        public async Task RemoveConnectionAsync(string connectionId)
        {
            var connection = (await _context.QueryAsync<SocketConnection>(connectionId).GetRemainingAsync()).SingleOrDefault();

            if (connection == null) throw new NullReferenceException($"Could not remove connectionId: {connectionId}. Socket Connection not found");

            // await _context.DeleteAsync<SocketConnection>(connectionId);
            await _context.DeleteAsync(connection);
            return;
        }

        public Task StoreConnectionAsync(string connectionId, string userId)
        {
            // _logger.LogInformation($"Storing connectionId: {connectionId}");

            var connection = new SocketConnection
            {
                ConnectionId = connectionId,
                UserId = userId
            };

            return _context.SaveAsync(connection);
        }
    }

    public class TableNameConstants
    {
        public const string SocketConnection = "SocketConnections";

    }
    [DynamoDBTable(TableNameConstants.SocketConnection)]
    public class SocketConnection
    {

        [DynamoDBHashKey]
        public string ConnectionId { get; set; }

        [DynamoDBRangeKey]
        public string UserId { get; set; }

        public List<string> GroupIds { get; set; }
    }

    // public class ConnectionGroup
    // {
    //     public string GroupId { get; set; }
    //     public string ConnectionId { get; set; }
    // }


}