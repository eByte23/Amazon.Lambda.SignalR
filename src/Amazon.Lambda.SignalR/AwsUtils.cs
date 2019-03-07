using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Leelou.Lea.Api.Services;
using Microsoft.AspNetCore.Builder;
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
            //services.AddScoped<IAWSWebSocketManager,AWSWebSocketManager>();
            services.AddScoped<IDynamoDBContext>(c => new DynamoDBContext(c.GetRequiredService<IAmazonDynamoDB>()));

            services
                .AddScoped<IHubClients, AWSHubCallerClients>()
                .AddScoped<IGroupManager, AWSSocketGroupManager>()
                .AddScoped<IAWSSocketConnectionStore<SocketConnection>, DynamoDbSocketConnectionStore>()
                .AddScoped<IAWSSocketManager, AWSSocketManager>()
                .AddScoped<HubCallerContext, AWSHubCallerContext>();


            return services;
        }

        public static IApplicationBuilder UseAWSWebsockets(this IApplicationBuilder app)
        {
            var feature = app.ServerFeatures.Get<IAWSWebSocketFeature>();
            if (feature != null)
            {
                app.Use(async (context, next) =>
                {
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
                });

            }


            return app;
        }
    }
}