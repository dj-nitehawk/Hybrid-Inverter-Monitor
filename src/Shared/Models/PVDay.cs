using System.Text.Json.Serialization;

namespace InverterMon.Shared.Models;

public class PVDay
{
    public int DayNumber { get; set; }
    public string DayName { get; set; }
    public decimal TotalKiloWattHours { get; set; }
    public IEnumerable<WattPeak> WattPeaks { get; set; }
    public int GraphTickCount { get; set; }
    public int[] GraphRange { get; set; }

    public class WattPeak
    {
        [JsonPropertyName("Time")]
        public string MinuteBucket { get; set; }

        [JsonPropertyName("Watts")]
        public int PeakWatt { get; set; }
    }
}