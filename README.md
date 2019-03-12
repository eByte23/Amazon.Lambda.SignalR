
### SignalR for AWS APIGatway Sockets


### What is this projects goal?

This projects goal is to be able to plug into Asp.Net Core Webapi on lambda running SignalR.
We would utilise one lambda func for all api and socket interactions.


### Stages

- [x] - Make an APIGateway and Websockets gateway work together happily
- [ ] - Investigate and use IWebSocketFeature 
- [ ] - Work on SignalR API's
- [ ] - Ask for help to make it better


### Useful repo links
https://github.com/aws/dotnet
https://github.com/aws/aws-lambda-dotnet



## Notes

When running websocket request through api we get errors in
https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.AspNetCoreServer/APIGatewayProxyFunction.cs

 protected override void MarshallRequest(InvokeFeatures features, APIGatewayProxyRequest apiGatewayRequest, ILambdaContext lambdaContext)
```
One or more errors occurred. (Object reference not set to an instance of an object.): AggregateException
at System.Threading.Tasks.Task`1.GetResultCore(Boolean waitCompletionNotification)
at lambda_method(Closure , Stream , Stream , LambdaContextInternal )

at Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction.MarshallRequest(InvokeFeatures features, APIGatewayProxyRequest apiGatewayRequest, ILambdaContext lambdaContext)
at Amazon.Lambda.AspNetCoreServer.AbstractAspNetCoreFunction`2.FunctionHandlerAsync(TREQUEST request, ILambdaContext lambdaContext)
at serverless.test.WarmAPIGatewayProxyFunction.FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext lambdaContext) in C:\Developer\ITFA\serverless.test\src\ServerlessTest\LambdaEntryPoint.cs:line 133
```
