namespace InverterMon.Shared.Models;

public class SystemSpec
{
    public int PVMaxCapacity { get; set; } = 10;
    public int BatteryCapacity { get; set; } = 100;
    public int SunlightStartHour { get; set; } = 6;
    public int SunlightEndHour { get; set; } = 18;
}