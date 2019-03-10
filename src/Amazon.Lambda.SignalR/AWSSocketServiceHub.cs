using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.Lambda.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Leelou.Lea.Api.Services
{
    public interface IAWSSocektService //: IHubContext<THub> //where THub : Hub
    {
        IHubClients Clients { get; set; }
        //
        // Summary:
        //     Gets or sets the hub caller context.
        HubCallerContext Context { get; set; }
        //
        // Summary:
        //     Gets or sets the group manager.
        IGroupManager Groups { get; set; }

        //
        void Dispose();
        //
        // Summary:
        //     Called when a new connection is established with the hub.
        //
        // Returns:
        //     A System.Threading.Tasks.Task that represents the asynchronous connect.

        Task OnMessageReceivedAsync(Stream content);
        Task OnConnectedAsync();
        //
        // Summary:
        //     Called when a connection with the hub is terminated.
        //
        // Returns:
        //     A System.Threading.Tasks.Task that represents the asynchronous disconnect.
        Task OnDisconnectedAsync(Exception exception);
        //
        // Summary:
        //     Releases all resources currently used by this Microsoft.AspNetCore.SignalR.Hub
        //     instance.
        //
        // Parameters:
        //   disposing:
        //     true if this method is being invoked by the Microsoft.AspNetCore.SignalR.Hub.Dispose
        //     method, otherwise false.
    }

    public abstract class AWSSockerServiceHub : IAWSSocektService, IDisposable //where THub :Hub
    {
        private HubCallerContext _hubCallerContext;
        public IGroupManager _groups;

        private IHubClients _clients;
        private bool _disposed;

        public AWSSockerServiceHub(HubCallerContext hubCallerContext, IGroupManager groups, IHubClients clients)
        {
            _hubCallerContext = hubCallerContext;
            _groups = groups;
            _clients = clients;
        }

        public IHubClients Clients
        {
            get
            {
                return _clients;
            }

            set
            {
                _clients = value;
            }
        }

        public IGroupManager Groups
        {
            get
            {
                return _groups;
            }

            set
            {
                _groups = value;
            }
        }

        public HubCallerContext Context
        {
            get
            {
                return _hubCallerContext;
            }
            set
            {
                _hubCallerContext = value;
            }
        }


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Dispose(true);

            _disposed = true;
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public virtual Task OnConnectedAsync()
        {

            return Task.CompletedTask;
        }


        public virtual Task OnDisconnectedAsync(Exception exception)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnMessageReceivedAsync(Stream content)
        {
            return Task.CompletedTask;
        }


        protected virtual void Dispose(bool disposing)
        {
        }
    }
}