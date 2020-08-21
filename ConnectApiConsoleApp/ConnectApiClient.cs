using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;


namespace ConnectApiConsoleApp
{
    public class ConnectApiClient
    {
        HttpClient _httpClient;

        const string dailyReport = "https://api.appstoreconnect.apple.com/v1/salesReports?filter[frequency]=DAILY&filter[reportSubType]=SUMMARY&filter[reportType]=SALES&filter[vendorNumber]={0}&filter[reportDate]={1}";
        private readonly string _vendor;

        public ConnectApiClient(string jwtToken, string vendor)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            _vendor = vendor;
        }

        public async Task<(bool, IEnumerable<ReportSummaryEntry>, string)> GetDailyReport(DateTime? date)
        {
            var reportDate = (date ?? DateTime.Now.AddDays(-1)).ToString("yyyy-MM-dd");

            var connectApiUrl = string.Format(dailyReport, _vendor, reportDate);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(connectApiUrl),
                Method = HttpMethod.Get
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/a-gzip"));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) return await ErroredResponse(response);

            return await GetRecords(response);

        }

        private async Task<(bool, IEnumerable<ReportSummaryEntry>, string)> GetRecords(HttpResponseMessage response)
        {
            var payload = await response.Content.ReadAsStreamAsync();

            var responseStream = new GZipStream(payload, CompressionMode.Decompress);
            TextReader reader = new StreamReader(responseStream);

            CsvConfiguration config = new CsvConfiguration(new System.Globalization.CultureInfo("en-US"))
            {
                Delimiter = "\t",
                HasHeaderRecord = true,
                HeaderValidated = null,

            };

            var csvReader = new CsvReader(reader, config);
            return (true, csvReader.GetRecords<ReportSummaryEntry>().ToList(), null);
        }

        private async Task<(bool, IEnumerable<ReportSummaryEntry>, string)> ErroredResponse(HttpResponseMessage response)
        {
            return (false, null, await response.Content.ReadAsStringAsync());
        }

        ~ConnectApiClient()
        {
            _httpClient.Dispose();
        }
    }



}
