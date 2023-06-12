using System.Net;
using System.Text.Json.Serialization;
using Dapr.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azure_functions_in_container_with_dapr
{
    public class DaprExample
    {
        private readonly ILogger _logger;
        private readonly string _daprStoreName = "function-statestore";

        public DaprExample(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DaprExample>();
        }

        [Function(nameof(SaveState))]
        public async Task<HttpResponseData> SaveState([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function [SaveState] processed a request.");

            var client = new DaprClientBuilder().Build();
            for (var i = 1; i <= 100; i++)
            {
                var order = new Order(i);

                // Save state into the state store
                await client.SaveStateAsync(_daprStoreName, i.ToString(), order.ToString());
                Console.WriteLine("Saving Order: " + order);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            await response.WriteStringAsync("State saved with Dapr on Azure Functions!");

            return response;
        }

        [Function(nameof(GetState))]
        public async Task<HttpResponseData> GetState([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function [GetState] processed a request.");

            var client = new DaprClientBuilder().Build();
            for (var i = 1; i <= 100; i++)
            {
                // Get state from the state store
                var result = await client.GetStateAsync<string>(_daprStoreName, i.ToString());
                Console.WriteLine("Getting Order: " + result);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            await response.WriteStringAsync("State retrieved with Dapr on Azure Functions!");

            return response;
        }

        [Function(nameof(DeleteState))]
        public async Task<HttpResponseData> DeleteState([HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function [DeleteState] processed a request.");

            var client = new DaprClientBuilder().Build();
            for (var i = 1; i <= 100; i++)
            {
                var order = new Order(i);

                // Delete state from the state store
                await client.DeleteStateAsync(_daprStoreName, i.ToString());
                Console.WriteLine("Deleting Order: " + order);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            await response.WriteStringAsync("State deleted with Dapr on Azure Functions!");

            return response;
        }
    }

    public record Order([property: JsonPropertyName("orderId")] int OrderId);
}
