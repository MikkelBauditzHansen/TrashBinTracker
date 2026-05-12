using System.Text.Json;

namespace TrashBinTracker.Service
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<double> GetCurrentTemperature()
        {
            string url =
                "https://api.open-meteo.com/v1/forecast" +
                "?latitude=55.6415" +
                "&longitude=12.0803" +
                "&current=temperature_2m";

            string json =
                await _httpClient.GetStringAsync(url);

            JsonDocument document =
                JsonDocument.Parse(json);

            double temperature =
                document.RootElement
                    .GetProperty("current")
                    .GetProperty("temperature_2m")
                    .GetDouble();

            return temperature;
        }
    }
}