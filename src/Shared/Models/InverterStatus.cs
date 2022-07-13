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
    public int PVInputWatt
    {
        get => pvInputWatt;
        set
        {
            pvInputWatt = value;
            if (value > 0)
            {
                double interval = (DateTime.Now - lastpvInputWattHourComputed).TotalSeconds;
                PVInputWattHour += value / (3600 / Convert.ToDecimal(interval));
            }
        }
    }
    public decimal PVInputWattHour
    {
        get => pvInputWattHour;
        set
        {
            pvInputWattHour = value;
            lastpvInputWattHourComputed = DateTime.Now;
        }
    }
    public decimal SCCVoltage { get; set; }
    public int BatteryDischargeCurrent { get; set; }
    public int BatteryDischargeWatts => BatteryDischargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryDischargeCurrent * BatteryVoltage);

    private int pvInputWatt;
    private decimal pvInputWattHour;
    private DateTime lastpvInputWattHourComputed;

    public void RestorePVWattHours(decimal accruedWattHours)
    {
        Console.WriteLine("restored: " + accruedWattHours);
        PVInputWattHour = accruedWattHours;
    }
}
