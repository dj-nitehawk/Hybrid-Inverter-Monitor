using System.Text.Json.Serialization;

namespace InverterMon.Shared.Models;

public class InverterStatus
{
    [JsonPropertyName("a")] public int BatteryCapacity { get; set; } = 200;
    [JsonPropertyName("b")] public decimal BatteryChargeCRate => BatteryChargeCurrent == 0 ? 0 : Convert.ToDecimal(BatteryChargeCurrent) / BatteryCapacity;
    [JsonPropertyName("c")] public int BatteryChargeCurrent { get; set; }
    [JsonPropertyName("d")] public int BatteryChargeWatts => BatteryChargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryChargeCurrent * BatteryVoltage);
    [JsonPropertyName("e")] public decimal BatteryDischargeCRate => BatteryDischargeCurrent == 0 ? 0 : Convert.ToDecimal(BatteryDischargeCurrent) / BatteryCapacity;
    [JsonPropertyName("f")] public int BatteryDischargeCurrent { get; set; }
    [JsonPropertyName("g")] public int BatteryDischargePotential => BatteryDischargeCurrent > 0 ? Convert.ToInt32(Convert.ToDouble(BatteryDischargeCurrent) / BatteryCapacity * 100) : 0;
    [JsonPropertyName("h")] public int BatteryDischargeWatts => BatteryDischargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryDischargeCurrent * BatteryVoltage);
    [JsonPropertyName("i")] public decimal BatteryVoltage { get; set; }
    [JsonPropertyName("j")] public int GridUsageWatts => GridVoltage < 10 ? 0 : LoadWatts + BatteryChargeWatts - (PVInputWatt + BatteryDischargeWatts);
    [JsonPropertyName("k")] public decimal GridVoltage { get; set; }
    [JsonPropertyName("l")] public int HeatSinkTemperature { get; set; }
    [JsonPropertyName("m")] public decimal LoadCurrent => LoadWatts == 0 ? 0 : LoadWatts / OutputVoltage;
    [JsonPropertyName("n")] public decimal LoadPercentage { get; set; }
    [JsonPropertyName("o")] public int LoadWatts { get; set; }
    [JsonPropertyName("p")] public decimal OutputVoltage { get; set; }
    [JsonPropertyName("q")] public decimal PVInputCurrent { get; set; }
    [JsonPropertyName("r")] public decimal PVInputVoltage { get; set; }
    [JsonPropertyName("s")]
    public int PVInputWatt
    {
        get => pvInputWatt;
        set
        {
            if (value > 0 && value != pvInputWatt)
            {
                pvInputWatt = value;
                double interval = (DateTime.Now - pvInputWattHourLastComputed).TotalSeconds;
                PVInputWattHour += value / (3600 / Convert.ToDecimal(interval));
                pvInputWattHourLastComputed = DateTime.Now;
            }
        }
    }
    [JsonPropertyName("t")] public decimal PVInputWattHour { get; private set; }
    [JsonPropertyName("u")] public int PV_MaxCapacity { get; set; }
    [JsonPropertyName("v")] public int PVPotential => PVInputWatt > 0 ? Convert.ToInt32(Convert.ToDouble(PVInputWatt) / PV_MaxCapacity * 100) : 0;

    private int pvInputWatt;
    private DateTime pvInputWattHourLastComputed;

    public void RestorePVWattHours(decimal accruedWattHours)
    {
        PVInputWattHour = accruedWattHours;
        pvInputWattHourLastComputed = DateTime.Now;
    }

    public void ResetPVWattHourAccumulation()
    {
        PVInputWattHour = 0;
        pvInputWattHourLastComputed = DateTime.Now;
    }
}