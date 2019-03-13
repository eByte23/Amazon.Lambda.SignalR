using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AwsDotnetCsharp
{
    public class Handler
    {
        public APIGatewayProxyResponse Hello(
           APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            //LogMessage(context, "Processing request started");
            APIGatewayProxyResponse response;
            try
            {
                //var result = processor.CurrentTimeUTC();
                response = CreateResponse(new{
                    data="gello"
                });

                //LogMessage(context, "Processing request succeeded.");
            }
            catch (Exception ex)
            {
               // LogMessage(context,
                 //   string.Format("Processing request failed - {0}", ex.Message));
                response = CreateResponse(new {});
            }

            return response;
        }

        APIGatewayProxyResponse CreateResponse<T>(T result)
        {
            int statusCode = (result != null) ?
                (int)HttpStatusCode.OK :
                (int)HttpStatusCode.InternalServerError;

            string body = (result != null) ?
                JsonConvert.SerializeObject(result) : string.Empty;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };
            return response;
        }
    }

}
