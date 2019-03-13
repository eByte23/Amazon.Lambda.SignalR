using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using Leelou.Lea.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amazon.Lambda.SignalR
{
    public static class Utils
    {
        // public static IServiceCollection AddDeveloperAwsOptions(this IServiceCollection services, IConfiguration config)
        // {
        //     var options = config.GetAWSOptions();

        //     string accessKey = config.GetSection("AWS").GetValue<string>("AccessKey");
        //     string accessSecret = config.GetSection("AWS").GetValue<string>("SecretKey");
        //     if (accessKey != null && accessSecret != null)
        //     {
        //         options.Credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, accessSecret);
        //     }

        //     services.AddDefaultAWSOptions(options);

        //     return services;
        // }

        // public static IServiceCollection AddDynamoDBWithDeveloperOptions(this IServiceCollection services, IConfiguration config)
        // {
        //     var dynamoDbConfig = config.GetSection("DynamoDb");
        //     var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");

        //     if (runLocalDynamoDb)
        //     {
        //         services.AddSingleton<IAmazonDynamoDB>(sp =>
        //         {
        //             var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl") };
        //             return new AmazonDynamoDBClient(clientConfig);
        //         });
        //     }
        //     else
        //     {
        //         services.AddAWSService<IAmazonDynamoDB>();
        //     }

        //     return services;
        // }

        public static IServiceCollection AddAWSWebsockets(this IServiceCollection services)
        {
            services
                .AddHttpContextAccessor()
                .AddAWSService<IAmazonDynamoDB>();

            services.AddScoped<IAmazonApiGatewayManagementApi>(c =>
            {
                var request = c.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["LambdaRequestObject"] as APIGatewayProxyRequest;
                var config = c.GetRequiredService<IConfiguration>();
                var websocketApiUrl = config["websocketapi"];
                var client = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig()
                {
                    ServiceURL = websocketApiUrl
                    //$"https://{request.RequestContext.DomainName}/{request.RequestContext.Stage}"
                });
                return client;
            });
            services.AddScoped<IDynamoDBContext>(c =>
            {
                var context = new DynamoDBContext(c.GetRequiredService<IAmazonDynamoDB>());


                return context;
            });

            services
                .AddScoped<IHubClients, AWSHubCallerClients>()
                .AddScoped<IGroupManager, AWSSocketGroupManager>()
                .AddScoped<IAWSSocketConnectionStore<SocketConnection>, DynamoDbSocketConnectionStore>()
                .AddScoped<IAWSSocketManager, AWSSocketManager>()
                .AddTransient<HubCallerContext, AWSHubCallerContext>();


            return services;
        }

        public static IApplicationBuilder UseAWSWebsockets(this IApplicationBuilder app)
        {

            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

            //This could be resolved as a singleton
            var tableName = config["SOCKETCONNECTIONS_TABLE"];

            if (!string.IsNullOrEmpty(tableName))
            {
                Console.WriteLine("table name:  " + tableName);
                AWSConfigsDynamoDB.Context.AddAlias(new TableAlias(TableNameConstants.SocketConnection, tableName));
            }

            app.Use(async (context, next) =>
            {
                //Do make use of the IWebSocketFeature in the runtime? Then we don't need this
                var request = context.Items["LambdaRequestObject"] as APIGatewayProxyRequest;

                if (string.IsNullOrEmpty(request?.RequestContext?.ConnectionId))
                {
                    await next();
                    return;
                }

                //Do make use of the IWebSocketFeature in the runtime? Then we don't need this
                var feature = new AWSWebSocketFeature
                {
                    ConnectionId = request.RequestContext.ConnectionId,
                    ConnectionAt = request.RequestContext.ConnectionAt,
                    MessageId = request.RequestContext?.MessageId,
                    RouteKey = request.RequestContext.RouteKey ?? "",
                    EventType = request.RequestContext.EventType,

                    DomainName = request.RequestContext?.DomainName ?? "",
                    ResourcePath = request.RequestContext?.ResourcePath ?? "",
                };

                context.Features.Set<IAWSWebSocketFeature>(feature);

                var hub = app.ApplicationServices.GetRequiredService<AWSSockerServiceHub>();
                //CONNECT, MESSAGE, or DISCONNECT
                if (feature.EventType == "CONNECT")
                {
                    await hub.OnConnectedAsync();
                }
                else if (feature.EventType == "DISCONNECT")
                {
                    await hub.OnDisconnectedAsync(new Exception());
                }
                else if (feature.EventType == "MESSAGE")
                {
                    await hub.OnMessageReceivedAsync(context.Request.Body);
                }
                else
                {
                    throw new Exception("Unhandled event type");
                }

                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("Connected");
                return;
            });

            return app;
        }
    }
}