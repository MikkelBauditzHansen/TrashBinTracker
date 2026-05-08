using Microsoft.AspNetCore.Mvc;

namespace TrashBinTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetWeather()
        {
            string url =
                "https://api.open-meteo.com/v1/forecast" +
                "?latitude=55.6415" +
                "&longitude=12.0803" +
                "&hourly=temperature_2m,precipitation,rain,showers,snowfall" +
                "&forecast_days=1";

            object? weatherData =
                await _httpClient.GetFromJsonAsync<object>(url);

            if (weatherData == null)
            {
                return StatusCode(500, "Could not fetch weather data");
            }

            return Ok(weatherData);
        }
    }
}