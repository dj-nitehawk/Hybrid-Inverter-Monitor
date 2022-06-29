namespace InverterMon.Shared.Models;

internal class InverterStatus
{
    public decimal GridVoltage { get; set; }
    public decimal GridFrequency { get; set; }
    public decimal OutputVoltage { get; set; }
    public int LoadWatts { get; set; }
    public decimal LoadPercentage { get; set; }
    public decimal BatteryVoltage { get; set; }
    public int BatteryChargeCurrent { get; set; }
    public int BatteryCapacity { get; set; }
    public int HeatSinkTemperature { get; set; }
    public decimal PVInputCurrent { get; private set; }
    public decimal PVInputVoltage { get; private set; }
    public int PVInputWatt { get; set; }
    public int BatteryDischargeCurrent { get; private set; }
    public char PVOrACFeed { get; private set; }
}
