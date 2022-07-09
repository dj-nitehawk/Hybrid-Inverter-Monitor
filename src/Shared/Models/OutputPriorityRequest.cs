namespace InverterMon.Shared.Models;

public class OutputPriorityRequest
{
    public const string SolarFirst = "01";
    public const string SolarBatteryUtility = "02";
    public const string UtilityFirst = "00";

    public string Priority { get; set; } = SolarFirst;
}
