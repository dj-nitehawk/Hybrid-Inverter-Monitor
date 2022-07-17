using InverterMon.Shared.Models;

namespace InverterMon.Server.Persistance.Settings;

public class UserSettings
{
    public int Id { get; set; } = 1;
    public int PVMaxCapacity { get; set; } = 10;
    public int SunlightStartHour { get; set; } = 6;
    public int SunlightEndHour { get; set; } = 18;

    public SystemSpec ToSystemSpec() => new()
    {
        PVMaxCapacity = PVMaxCapacity,
        SunlightStartHour = SunlightStartHour,
        SunlightEndHour = SunlightEndHour
    };

    public void FromSystemSpec(SystemSpec spec)
    {
        Id = 1;
        PVMaxCapacity = spec.PVMaxCapacity;
        SunlightStartHour = spec.SunlightStartHour;
        SunlightEndHour = spec.SunlightEndHour;
    }
}