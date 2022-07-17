namespace InverterMon.Shared.Models;

public class InverterStatus
{
    public int GridUsageWatts => GridVoltage < 10 ? 0 : LoadWatts + BatteryChargeWatts - (PVInputWatt + BatteryDischargeWatts);
    public decimal GridVoltage { get; set; }
    public decimal OutputVoltage { get; set; }
    public int LoadWatts { get; set; }
    public decimal LoadCurrent => LoadWatts == 0 ? 0 : LoadWatts / OutputVoltage;
    public decimal LoadPercentage { get; set; }
    public decimal BatteryVoltage { get; set; }
    public int BatteryChargeCurrent { get; set; }
    public int BatteryChargeWatts => BatteryChargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryChargeCurrent * BatteryVoltage);
    public int HeatSinkTemperature { get; set; }
    public decimal PVInputCurrent { get; set; }
    public decimal PVInputVoltage { get; set; }
    public int PVMaxCapacity { get; set; } = 1;
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
    public decimal PVInputWattHour { get; private set; }
    public int PVPotential => PVInputVoltage > 0 ? Convert.ToInt32(PVInputCurrent / PVMaxCapacity * 100) : 0;
    public decimal SCCVoltage { get; set; }
    public int BatteryDischargeCurrent { get; set; }
    public int BatteryDischargeWatts => BatteryDischargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryDischargeCurrent * BatteryVoltage);

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
