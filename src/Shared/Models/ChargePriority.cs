namespace InverterMon.Shared.Models;

public static class ChargePriority
{
    public const string SolarFirst = "01";
    public const string SolarAndUtility = "02";
    public const string OnlySolar = "03";
    public const string UtilityFirst = "00";
}
