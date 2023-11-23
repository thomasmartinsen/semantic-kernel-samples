namespace Models;

public class WeatherData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double GenerationTime { get; set; }
    public int UtcOffsetSeconds { get; set; }
    public string Timezone { get; set; }
    public string TimezoneAbbreviation { get; set; }
    public double Elevation { get; set; }
    public CurrentWeatherData CurrentWeather { get; set; }
}
