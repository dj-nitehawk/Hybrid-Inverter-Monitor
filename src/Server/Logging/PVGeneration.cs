namespace InverterMon.Server.Logging;

public class PVGeneration
{
    public int Id { get; set; }
    public Dictionary<int, int> HourlyMaximums { get; set; } = new();
    public decimal TotalWattHours { get; set; }

    public void SetHourlyMaximum(int newValue)
    {
        var currentHour = DateTime.Now.Hour;

        if (currentHour is >= 6 and <= 18)
        {
            if (HourlyMaximums.ContainsKey(currentHour))
            {
                if (HourlyMaximums[currentHour] < newValue)
                    HourlyMaximums[currentHour] = newValue;
            }
            else
            {
                HourlyMaximums[currentHour] = newValue;
            }
        }
    }

    public void SetTotalWattHours(decimal totalWattHours)
    {
        TotalWattHours = totalWattHours;
        Console.WriteLine("updated: " + totalWattHours);
    }
}