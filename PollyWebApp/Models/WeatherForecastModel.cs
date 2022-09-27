namespace PollyWebApp.Models
{
    using System.Text.Json.Serialization;

    public class WeatherForecastModel
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("temperatureC")]
        public int TemperatureC { get; set; }
        [JsonPropertyName("temperatureF")]
        public int TemperatureF { get; set; }
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;
    }
}
