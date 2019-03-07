using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting.Internal;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.SignalR;
using Microsoft.AspNetCore.Http.Features;

namespace serverless.test
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the
    /// actual Lambda function entry point. The Lambda handler field should be set to
    ///
    /// LeaWS::LeaWS.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint : WarmAPIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .CaptureStartupErrors(true)
                .UseStartup<Startup>();
        }
    }


    public abstract class WarmAPIGatewayProxyFunction : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {

        // protected override void PostCreateContext(HostingApplication.Context context, APIGatewayProxyRequest lambdaRequest, ILambdaContext lambdaContext)
        // {
        //     if (!string.IsNullOrEmpty(lambdaRequest.RequestContext.ConnectionId))
        //     {
        //          Console.WriteLine("gere");
        //         context.HttpContext.Features.Set<IAWSWebSocketFeature>(new AWSWebSocketFeature
        //         {
        //             ConnectionId = lambdaRequest.RequestContext.ConnectionId,
        //             ConnectionAt = lambdaRequest.RequestContext.ConnectionAt,
        //             MessageId = lambdaRequest.RequestContext?.MessageId,
        //             RouteKey = lambdaRequest.RequestContext.RouteKey ?? "",
        //             EventType = lambdaRequest.RequestContext.EventType,

        //             DomainName = lambdaRequest.RequestContext?.DomainName ?? "",
        //             ResourcePath = lambdaRequest.RequestContext?.ResourcePath ?? "",

        //         });
        //     }

        //     base.PostCreateContext(context,lambdaRequest,lambdaContext);
        // }

        // protected override void MarshallRequest(InvokeFeatures features, APIGatewayProxyRequest apiGatewayRequest, ILambdaContext lambdaContext)
        // {
        //     if (!string.IsNullOrEmpty(apiGatewayRequest.RequestContext.ConnectionId))
        //     {
        //         if (features == null)
        //         {
        //             Console.WriteLine("null");
        //             features = new InvokeFeatures();
        //         }

        //         Console.WriteLine("gere");
        //         features[typeof(IAWSWebSocketFeature)] = new AWSWebSocketFeature
        //         {
        //             ConnectionId = apiGatewayRequest.RequestContext.ConnectionId,
        //             ConnectionAt = apiGatewayRequest.RequestContext.ConnectionAt,
        //             MessageId = apiGatewayRequest.RequestContext?.MessageId,
        //             RouteKey = apiGatewayRequest.RequestContext.RouteKey ?? "",
        //             EventType = apiGatewayRequest.RequestContext.EventType,

        //             DomainName = apiGatewayRequest.RequestContext?.DomainName ?? "",
        //             ResourcePath = apiGatewayRequest.RequestContext?.ResourcePath ?? "",

        //         };
        //     }
        //     base.MarshallRequest(features, apiGatewayRequest, lambdaContext);
        // }

        public override async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            // Console.WriteLine("In overridden FunctionHandlerAsync");

            // if (!string.IsNullOrEmpty(request.RequestContext.ConnectionId))
            // {
            //     Console.WriteLine(request.RequestContext.ConnectionId ?? "");
            //     Console.WriteLine(request.RequestContext.ConnectionAt.ToString());
            //     Console.WriteLine(request.RequestContext.RouteKey ?? "");
            //     Console.WriteLine(request.RequestContext.ResourcePath ?? "");
            //     Console.WriteLine(request.RequestContext.MessageId ?? "");
            //     Console.WriteLine(request.RequestContext.DomainName ?? "");
            //     Console.WriteLine(request.RequestContext.EventType ?? "");
            // }

            if (request.Resource == "WarmingLambda")
            {
                var concurrencyCount = 1;
                int.TryParse(request.Body, out concurrencyCount);

                if (concurrencyCount > 1)
                {
                    Console.WriteLine($"Warming instance { concurrencyCount}.");
                    var client = new AmazonLambdaClient();
                    await client.InvokeAsync(new Amazon.Lambda.Model.InvokeRequest
                    {
                        FunctionName = lambdaContext.FunctionName,
                        InvocationType = InvocationType.RequestResponse,
                        Payload = JsonConvert.SerializeObject(new APIGatewayProxyRequest
                        {
                            Resource = request.Resource,
                            Body = (concurrencyCount - 1).ToString()
                        })
                    });
                }

                return new APIGatewayProxyResponse { };
            }


            return await base.FunctionHandlerAsync(request, lambdaContext);
        }
    }
}
