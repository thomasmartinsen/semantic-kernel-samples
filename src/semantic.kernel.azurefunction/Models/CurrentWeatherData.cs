namespace Models;

public class CurrentWeatherData
{
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public int WindDirection { get; set; }
    public int WeatherCode { get; set; }
    public int IsDay { get; set; }
    public string Time { get; set; }
}
