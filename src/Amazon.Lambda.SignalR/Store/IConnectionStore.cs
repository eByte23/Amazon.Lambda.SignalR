using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.Lambda.SignalR
{
    public interface IAWSSocketConnectionStore<T>
    {
        Task AddConnectionToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken));
        Task RemoveConnectionFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default(CancellationToken));


        Task StoreConnection(string connectionId, string userId);
        Task RemoveConnection(string connectionId);

        Task<List<T>> GetGroup(string groupName);

        //List<SocketClient>
        Task<List<T>> GetClient(string connectionId);
        Task<List<T>> GetAllClients();
        Task<List<T>> GetClients(IReadOnlyList<string> connectionIds);
        Task<List<T>> GetAllClientsExcept(IReadOnlyList<string> connectionIds);
        // Task<List<SocketClient>> GetAllClientsExcept();

        Task<List<T>> GetUser(string userId);
        Task<List<T>> GetUsers(IReadOnlyList<string> userIds);
    }

}