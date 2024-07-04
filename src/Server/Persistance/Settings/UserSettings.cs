using InverterMon.Server.Persistance.PVGen;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Persistance.Settings;

public class UserSettings
{
    public int Id { get; set; } = 1;
    public int PV_MaxCapacity { get; set; } = 1000;
    public int BatteryCapacity { get; set; } = 100;
    public int SunlightStartHour { get; set; } = 6;
    public int SunlightEndHour { get; set; } = 18;
    public int[] PVGraphRange => [0, (SunlightEndHour - SunlightStartHour) * 60];
    public int PVGraphTickCount => PVGraphRange[1] / (int)PVGenExtensions.BucketDuration.TotalMinutes;

    public SystemSpec ToSystemSpec()
        => new()
        {
            PV_MaxCapacity = PV_MaxCapacity,
            BatteryCapacity = BatteryCapacity,
            SunlightStartHour = SunlightStartHour,
            SunlightEndHour = SunlightEndHour
        };

    public void FromSystemSpec(SystemSpec spec)
    {
        Id = 1;
        PV_MaxCapacity = spec.PV_MaxCapacity;
        BatteryCapacity = spec.BatteryCapacity;
        SunlightStartHour = spec.SunlightStartHour;
        SunlightEndHour = spec.SunlightEndHour;
    }
}