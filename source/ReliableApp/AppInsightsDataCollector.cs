

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReliableApp
{
    public class AppInsightsDataCollector
    {
        private static HttpClient client = new()
        {
            BaseAddress = new Uri("https://api.applicationinsights.io/")
        };
        private static string errorBudgetQuery = @" requests | where timestamp > ago({0}) | summarize succeed=count(success == true), failedCount=count(success == false), totalCount=sum(success == true or success == false) by timestamp  | extend SLI = succeed * 100.00 / totalCount | summarize ErrorBudget = (avg(SLI) - 70) | project ErrorBudget";
        private static string Key = "vtav0kkn3d3h6552x062mw30o3x2i1fvxhdjdcgm";
        private static string ID = "ce06d217-5978-4d88-b28a-4a6502fd0c50";

        public static async Task<RootErrorBudget> GetErrorBudgetAsync(string timespan = "")
        {
            timespan = string.IsNullOrWhiteSpace(timespan) ? "200m" : timespan;
            var request = new HttpRequestMessage(HttpMethod.Post, $"/v1/apps/{ID}/query");
            request.Headers.Add("x-api-key", Key);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { query = string.Format(errorBudgetQuery, timespan) }), 
                Encoding.UTF8, "application/json");

            var body = default(RootErrorBudget);
            var response = await client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                body = await JsonSerializer.DeserializeAsync<RootErrorBudget>(
                    await response.Content.ReadAsStreamAsync());
            }
            return body;
        }

        public class ColumnrrorBudget
        {
            public string name { get; set; }
            public string type { get; set; }
        }

        public class TablerrorBudget
        {
            public string name { get; set; }
            public List<ColumnrrorBudget> columns { get; set; }
            public List<List<double>> rows { get; set; }
        }

        public class RootErrorBudget
        {
            public List<TablerrorBudget> tables { get; set; }

            public int GetErrorBudget()
            {
                return Math.Abs((int)GetValue());
            }

            public int GetBurnedOut()
            {
                return Math.Abs(100 - GetErrorBudget());
            }

            public double GetValue()
            {
                try
                {
                    return tables[0].rows[0][0];
                }
                catch { }
                return 0;
            }
        }


    }
}
