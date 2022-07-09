namespace InverterMon.Shared.Models;

public class ChargePriorityRequest
{
    public const string SolarFirst = "01";
    public const string SolarAndUtility = "02";
    public const string OnlySolar = "03";
    public const string UtilityFirst = "00";

    public string Priority { get; set; } = SolarFirst;
}
