using System;
using Xunit;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;

namespace Amazon.Lambda.SignalR.Tests
{
    public class Fixture
    {
        public IDynamoDBContext Context { get; private set; }
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public Fixture()
        {
            var clientConfig = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:8000" };
            _amazonDynamoDb = new AmazonDynamoDBClient("local", "local", clientConfig);
            Context = new DynamoDBContext(_amazonDynamoDb);
        }
    }
    public class UnitTest1 : IClassFixture<Fixture>
    {
        Fixture fixture;
        private readonly IAWSSocketConnectionStore<SocketConnection> _connectionStore;
        public const string connectionIdA = "abc";
        public const string connectionIdB = "def";
        public const string groupName = "Police";
        public const string userIdA = "123";
        public const string userIdB = "456";
        public UnitTest1(Fixture fixture)
        {
            this.fixture = fixture;
            this._connectionStore = new DynamoDbSocketConnectionStore(fixture.Context, null);
        }

        [Fact]
        public async void CanAddConnectionToGroupAsync()
        {
            var connection = new SocketConnection
            {
                ConnectionId = connectionIdA,
                UserId = userIdA
            };

            await fixture.Context.SaveAsync(connection);
            await _connectionStore.AddConnectionToGroupAsync(connectionIdA, groupName);
            var connect = (await _connectionStore.GetClient(connectionIdA)).SingleOrDefault();
            Assert.NotNull(connect);
            Assert.True(connect.GroupIds.Contains(groupName));
        }

        [Fact]
        public async void CanRemoveConnectionFromGroupAsync()
        {
            var connection = new SocketConnection
            {
                ConnectionId = connectionIdA,
                UserId = userIdA
            };

            await fixture.Context.SaveAsync(connection);
            await _connectionStore.RemoveConnectionFromGroupAsync(connectionIdA, groupName);
            var connect = (await _connectionStore.GetClient(connectionIdA)).SingleOrDefault();
            Assert.NotNull(connect);
            if (connection.GroupIds != null)
            {
                Assert.True(!connect.GroupIds.Contains(groupName));
            }
        }

        [Fact]
        public async void CanStoreConnectionAsync()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);

            Assert.NotNull(_connectionStore.GetClient(connectionIdA));
        }

        [Fact]
        public async void CanRemoveConnectionAsync()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);
            await _connectionStore.RemoveConnectionAsync(connectionIdA);

            Assert.True(!(await _connectionStore.GetAllClients()).Select(x => x.ConnectionId).Contains(connectionIdA));
        }

        [Fact]
        public async void CanGetGroup()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);
            await _connectionStore.AddConnectionToGroupAsync(connectionIdA, groupName);

            Assert.Equal(groupName, (await _connectionStore.GetGroup(groupName)).SingleOrDefault().GroupIds.SingleOrDefault());
        }

        [Fact]
        public async void CanGetClient()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);

            Assert.Equal(connectionIdA, (await _connectionStore.GetClient(connectionIdA)).SingleOrDefault().ConnectionId);
        }

        [Fact]
        public async void CanGetAllClients()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);
            await _connectionStore.StoreConnectionAsync(connectionIdB, userIdA);

            Assert.True((await _connectionStore.GetAllClients()).Select(x => x.ConnectionId).Contains(connectionIdA));
            Assert.True((await _connectionStore.GetAllClients()).Select(x => x.ConnectionId).Contains(connectionIdB));
        }

        [Fact]
        public async void CanGetClients()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);
            await _connectionStore.StoreConnectionAsync(connectionIdB, userIdA);
            var connectionIds = new List<string> { connectionIdA, connectionIdB };

            Assert.True((await _connectionStore.GetClients(connectionIds)).Select(x => x.ConnectionId).Contains(connectionIdA));
            Assert.True((await _connectionStore.GetClients(connectionIds)).Select(x => x.ConnectionId).Contains(connectionIdB));
        }

        [Fact]
        public async void CanGetAllClientsExcept()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);
            await _connectionStore.StoreConnectionAsync(connectionIdB, userIdA);
            var connectionIds = new List<string> { connectionIdA };

            Assert.True((await _connectionStore.GetAllClientsExcept(connectionIds)).Select(x => x.ConnectionId).Contains(connectionIdB));
            Assert.True(!(await _connectionStore.GetAllClientsExcept(connectionIds)).Select(x => x.ConnectionId).Contains(connectionIdA));
        }

        [Fact]
        public async void CanGetUser()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);

            Assert.Equal(userIdA, (await _connectionStore.GetUser(userIdA)).SingleOrDefault().UserId);
        }

        [Fact]
        public async void CanGetUsers()
        {
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdA);
            await _connectionStore.StoreConnectionAsync(connectionIdA, userIdB);
            var userIds = new List<string> { userIdA, userIdB };

            Assert.True((await _connectionStore.GetUsers(userIds)).Select(x => x.UserId).Contains(userIdA));
            Assert.True((await _connectionStore.GetUsers(userIds)).Select(x => x.UserId).Contains(userIdB));
        }
    }
}
