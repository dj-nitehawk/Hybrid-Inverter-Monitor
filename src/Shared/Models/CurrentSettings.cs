namespace InverterMon.Shared.Models;

public class CurrentSettings
{
    public string ChargePriority { get; set; }
    public string OutputPriority { get; set; }
    public string MaxACChargeCurrent { get; set; }
    public string MaxCombinedChargeCurrent { get; set; }

    public SystemSpec SystemSpec { get; set; } = new();
}
