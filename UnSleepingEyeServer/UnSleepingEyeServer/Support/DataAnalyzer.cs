using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnSleepingEyeServer.Support
{
    public class DataAnalyzer
    {
        static string _path = "http://127.0.0.1:8000";
        HttpClient _api_client;
        public DataAnalyzer()
        {
            _api_client = new HttpClient();
            _api_client.BaseAddress = new Uri(_path);
        }
        public IEnumerable<List<string>> GetKeyWordsForText(string[] text)
        {
            var jsonString = JsonSerializer.Serialize<string[]>(text);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var result = _api_client.PostAsync("ReportAnalyze", content);
            var resultContent = JsonSerializer.Deserialize(result.Result.Content.ReadAsStringAsync().Result, typeof(List<List<string>>));
            foreach (var data in (List<List<string>>)resultContent)
                yield return data;
        }
        public IEnumerable<List<string>> GetKeyWordsForText(string text)
        {
            return GetKeyWordsForText(text.Split(new char[] { '\n', '\r' }));
        }
        public async Task<int> GetSolitionID(OperationReport current, OperationReport[] solutions)
        {
            var array = new object[2];
            var cur_dict = new Dictionary<string, string>();
            var report_dict = new Dictionary<long, Dictionary<string, string>>();
            foreach (var prop in current.ReportData)
                cur_dict.Add(prop.Name, prop.Value);
            foreach (var sol in solutions)
            {
                var cursor = new Dictionary<string, string>();
                foreach (var data in sol.ReportData)
                    cursor.Add(data.Name, data.Value);
                report_dict.Add(sol.ID, cursor);
            }
            var jsonString = JsonSerializer.Serialize(report_dict);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var result = await _api_client.PostAsync("Solution", content);
            var resultContent = JsonSerializer.Deserialize(result.Content.ReadAsStringAsync().Result, typeof(int));
            return (int)resultContent;
        }

    }
}
