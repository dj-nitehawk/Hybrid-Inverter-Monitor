using InverterMon.Shared.Models;

namespace InverterMon.Server.Persistance.Settings;

public class UserSettings
{
    public int Id { get; set; } = 1;
    public int PVMaxCapacity { get; set; } = 10;
    public int BatteryCapacity { get; set; } = 100;
    public int SunlightStartHour { get; set; } = 6;
    public int SunlightEndHour { get; set; } = 18;

    public SystemSpec ToSystemSpec() => new()
    {
        PVMaxCapacity = PVMaxCapacity,
        BatteryCapacity = BatteryCapacity,
        SunlightStartHour = SunlightStartHour,
        SunlightEndHour = SunlightEndHour
    };

    public void FromSystemSpec(SystemSpec spec)
    {
        Id = 1;
        PVMaxCapacity = spec.PVMaxCapacity;
        BatteryCapacity = spec.BatteryCapacity;
        SunlightStartHour = spec.SunlightStartHour;
        SunlightEndHour = spec.SunlightEndHour;
    }
}