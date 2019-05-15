using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;

namespace NumberTaker.Forms
{
    public class AzureService
    {
        HttpClient client;

        public async Task UploadPhoto(MediaFile photo)
        {
            client = client ?? new HttpClient();

            using (var photoStream = photo.GetStream())
            {
                var bytes = new byte[photoStream.Length];
                await photoStream.ReadAsync(bytes, 0, Convert.ToInt32(photoStream.Length));

                var content = new
                {
                    Photo = Convert.ToBase64String(bytes)
                };

                var jsonObject = JToken.FromObject(content);
                var json = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");

                await client.PostAsync("https://number-taker-functions.azurewebsites.net/api/ProcessPhoto", json);
            }
        }
    }
}
