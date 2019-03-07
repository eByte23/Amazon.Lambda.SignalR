namespace Amazon.Lambda.SignalR
{
    public interface IAWSWebSocketFeature
    {
        //
        // Summary:
        //     A unique server-side ID for a message. Available only when the $context.eventType
        //     is MESSAGE.
        //     This field is only set for WebSocket API requests.
        string MessageId { get; set; }
        //
        // Summary:
        //     The event type: CONNECT, MESSAGE, or DISCONNECT.
        //     This field is only set for WebSocket API requests.
        string EventType { get; set; }
        //
        // Summary:
        //     A domain name for the WebSocket API. This can be used to make a callback to the
        //     client (instead of a hard-coded value).
        //     This field is only set for WebSocket API requests.
        string DomainName { get; set; }
        //
        // Summary:
        //     The Epoch-formatted connection time in a WebSocket API.
        //     This field is only set for WebSocket API requests.
        long ConnectionAt { get; set; }
        //
        // Summary:
        //     The connectionId identifies a unique client connection in a WebSocket API.
        //     This field is only set for WebSocket API requests.
        string ConnectionId { get; set; }
        //
        // Summary:
        //     An automatically generated ID for the API call, which contains more useful information
        //     for debugging/troubleshooting.
        string ExtendedRequestId { get; set; }
        //
        // Summary:
        //     The resource path defined in API Gateway
        //     This field is only set for REST API requests.
        string ResourcePath { get; set; }
        //
        // Summary:
        //     The unique request id
        string RequestId { get; set; }
        //
        // Summary:
        //     The API Gateway stage name
        string Stage { get; set; }
        //
        // Summary:
        //     The resource id.
        string ResourceId { get; set; }
        //
        // Summary:
        //     The resource full path including the API Gateway stage
        //     This field is only set for REST API requests.
        string Path { get; set; }
        //
        // Summary:
        //     The selected route key.
        //     This field is only set for WebSocket API requests.
        string RouteKey { get; set; }
    }


    public class AWSWebSocketFeature : IAWSWebSocketFeature
    {
        public string MessageId { get; set; }
        public string EventType { get; set; }
        public string DomainName { get; set; }
        public long ConnectionAt { get; set; }
        public string ConnectionId { get; set; }
        public string ExtendedRequestId { get; set; }
        public string ResourcePath { get; set; }
        public string RequestId { get; set; }
        public string Stage { get; set; }
        public string ResourceId { get; set; }
        public string Path { get; set; }
        public string RouteKey { get; set; }
    }
}