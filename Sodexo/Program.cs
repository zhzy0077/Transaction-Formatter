using System;

namespace Sodexo
{
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    internal sealed class Program
    {
        static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var httpClient = new HttpClient();

            using var payload = new StringContent(JsonConvert.SerializeObject(new
            {
                mobile = DotNetEnv.Env.GetString("MOBILE"),
                password = DotNetEnv.Env.GetString("PASSWORD"),
            }), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://epass.sdxpass.com/server/authenticator/signIn", payload);

            string token = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync()).authToken;

            using var paging = new StringContent(JsonConvert.SerializeObject(new
            {
                currentPageNumber = 1,
                cardId = "",
                fromDate = DotNetEnv.Env.GetString("FROM_DATE"),
                toDate = DotNetEnv.Env.GetString("TO_DATE")
            }), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://epass.sdxpass.com/server/ewallet/getTransactions"),
                Method = HttpMethod.Post,
                Headers =
                {
                    {"auth-token", token}
                },
                Content = paging
            };
            response = await httpClient.SendAsync(request);

            await using var writer = new FileStream("out.json", FileMode.OpenOrCreate);
            await response.Content.CopyToAsync(writer);
        }
    }
}