namespace InverterMon.Shared.Models;

public class InverterStatus
{
    public int GridUsageWatts => LoadWatts + BatteryChargeWatts - (PVInputWatt + BatteryDischargeWatts);
    //public decimal GridVoltage { get; set; }
    //public decimal GridFrequency { get; set; }
    public decimal OutputVoltage { get; set; }
    //public decimal OutputFrequency { get; set; }
    //public int LoadVA { get; set; }
    public int LoadWatts
    {
        get => loadWatts;
        set
        {
            if (value != loadWatts)
            {
                loadWatts = value;
                //double interval = (DateTime.Now - lastloadWattHourComputed).TotalSeconds;
                //LoadWattHours = value / (3600 / Convert.ToDecimal(interval));
            }
        }
    }
    public decimal LoadCurrent
    {
        get
        {
            return loadWatts == 0 ? 0 : LoadWatts / OutputVoltage;
        }
    }

    //public decimal LoadWattHours
    //{
    //    get => loadWattHours;
    //    set
    //    {
    //        if (value != loadWattHours && lastloadWattHourComputed != new DateTime())
    //        {
    //            loadWattHours = value;
    //        }

    //        lastloadWattHourComputed = DateTime.Now;
    //    }
    //}
    //public decimal LoadPercentage { get; set; }
    //public decimal BusVoltage { get; set; }
    public decimal BatteryVoltage { get; set; }
    public int BatteryChargeCurrent { get; set; }
    public int BatteryChargeWatts
    {
        get
        {
            return BatteryChargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryChargeCurrent * BatteryVoltage);
        }
    }

    //public int BatteryCapacity { get; set; }
    public int HeatSinkTemperature { get; set; }
    public decimal PVInputCurrent { get; set; }
    public decimal PVInputVoltage { get; set; }
    public int PVInputWatt
    {
        get => pvInputWatt;
        set
        {
            if (value != pvInputWatt)
            {
                pvInputWatt = value;
                //double interval = (DateTime.Now - lastpvInputWattHourComputed).TotalSeconds;
                //PVInputWattHour = value / (3600 / Convert.ToDecimal(interval));
            }
        }
    }
    //public decimal PVInputWattHour
    //{
    //    get => pvInputWattHour; set
    //    {
    //        if (value != pvInputWattHour && lastpvInputWattHourComputed != new DateTime())
    //        {
    //            pvInputWattHour = value;
    //        }
    //        lastpvInputWattHourComputed = DateTime.Now;

    //    }
    //}    
    //public decimal SCCVoltage { get; set; }
    public int BatteryDischargeCurrent { get; set; }
    public int BatteryDischargeWatts
    {
        get
        {
            return BatteryDischargeCurrent == 0 ? 0 : Convert.ToInt32(BatteryDischargeCurrent * BatteryVoltage);
        }
    }

    //public char PVOrACFeed { get; set; }
    //public char LoadOn { get; set; }
    //public char SCCOn { get; set; }
    //public char ACChargeOn { get; set; }

    private int loadWatts;
    //private decimal loadWattHours;
    private int pvInputWatt;
    //private DateTime lastloadWattHourComputed;
    //private decimal pvInputWattHour;
    //private DateTime lastpvInputWattHourComputed;
}
