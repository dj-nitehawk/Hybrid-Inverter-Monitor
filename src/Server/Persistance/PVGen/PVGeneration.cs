namespace InverterMon.Server.Persistance.PVGen;

public class PVGeneration
{
    public int Id { get; set; }
    public Dictionary<string, int> WattPeaks { get; set; } = new();
    public decimal TotalWattHours { get; set; }

    public void SetWattPeaks(int newValue)
    {
        var currentHour = DateTime.Now.Hour;

        if (currentHour is >= 7 and <= 17)
        {
            var key = DateTime.Now.ToKey();

            if (WattPeaks.ContainsKey(key))
            {
                if (WattPeaks[key] < newValue)
                    WattPeaks[key] = newValue;
            }
            else
            {
                WattPeaks[key] = newValue;
            }
        }
    }

    public void SetTotalWattHours(decimal totalWattHours)
    {
        TotalWattHours = totalWattHours;
    }
}
