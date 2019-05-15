using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Documents;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NumberTaker
{
    public class NumberItem
    {
        [JsonProperty("id")]
        public string Id {get; set;}

        public double Number {get; set;}
    }

    public static class ProcessPhoto
    {
        private const int numberOfCharsInOperationId = 36;

        private static string SubscriptionKey = Environment.GetEnvironmentVariable("SubscriptionKey");
        static ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(SubscriptionKey))
            {
                Endpoint = "https://uksouth.api.cognitive.microsoft.com/"
            };

        [FunctionName("ProcessPhoto")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Photos",
                collectionName: "Text",
                ConnectionStringSetting = "CosmosDBConnection")]  IAsyncCollector<NumberItem> items,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var data = await req.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(data);
            string photo = obj?.Photo;
            var imageBytes = Convert.FromBase64String(photo);

            using (var ms = new MemoryStream(imageBytes))
            {
                var result = await computerVision.RecognizeTextInStreamAsync(ms, TextRecognitionMode.Handwritten);
                var text = await GetTextAsync(result.OperationLocation);
                
                log.LogInformation($"Text read = {text}");

                if (double.TryParse(text, out var d))
                {
                    await items.AddAsync(new NumberItem{Id = Guid.NewGuid().ToString(), Number = d});
                }

                return new OkResult();
            }
        }

        static readonly HttpClient httpClient = new HttpClient();
        static readonly string tempThresholdUrl = $"https://fan-controller.azurewebsites.net/api/temp-threshold/fancontroller";

        [FunctionName("CosmosDBTrigger")]
        public static async Task CosmosDBTrigger(
            [CosmosDBTrigger(
                databaseName: "Photos",
                collectionName: "Text",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> items,
            ILogger log)
        {
            log.LogInformation("Cosmos DB trigger function processed a request.");

            var data = new
            {
                threshold = items.Last().GetPropertyValue<double>("Number")
            };
            
            log.LogInformation($"Sending threshold = {data.threshold}");

            var serialized = JsonConvert.SerializeObject(data);
            var content = new StringContent(serialized, Encoding.ASCII, "application/json");
            await httpClient.PostAsync(tempThresholdUrl, content);
        }

        private static async Task<string> GetTextAsync(string operationLocation)
        {
            // Retrieve the URI where the recognized text will be
            // stored from the Operation-Location header
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            var result = await computerVision.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                await Task.Delay(1000);
                result = await computerVision.GetTextOperationResultAsync(operationId);
            }

            // Return the results
            var lines = result.RecognitionResult.Lines;
            return string.Join(Environment.NewLine, lines.Select(l => l.Text));
        }
    }
}
