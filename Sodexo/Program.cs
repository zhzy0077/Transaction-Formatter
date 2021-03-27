using System;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper;

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
                mobile = long.Parse(DotNetEnv.Env.GetString("MOBILE")),
                password = DotNetEnv.Env.GetString("PASSWORD"),
            }), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://epass.sdxpass.com/server/authenticator/signIn", payload);

            var s = await response.Content.ReadAsStringAsync();
            string token = JsonConvert.DeserializeObject<dynamic>(s).authToken;

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

            var transactions = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            await using var writer = new StreamWriter("sodexo.csv");
            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            foreach (var trans in transactions.transactionData)
            {
                string transactionDate = trans.TransactionDate;
                decimal amount = trans.amount;
                string description = trans.merchantName;
                string id = trans.transactionCode;
                if (trans.TransType == 1)
                {
                    amount = -amount;
                }
                var t = new
                {
                    TransactionDate = transactionDate,
                    Amount = amount,
                    Description = description,
                    Id = id,
                    Account = description,
                };
                await csvWriter.WriteRecordsAsync(new List<object>
                {
                    t
                });
            }
        }
    }
}