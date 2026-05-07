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
        public async Task<ActionResult<object>> GetWeather(
     [FromQuery] double latitude,
     [FromQuery] double longitude)
        {
            string url =
                $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

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
