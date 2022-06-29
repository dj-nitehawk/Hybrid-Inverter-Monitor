namespace InverterMon.Server;

internal class Status : CommandBase
{
    protected override string Command => "QPIGS";

    public decimal GridVoltage { get; private set; }
    public decimal GridFrequency { get; private set; }
    public decimal OutputVoltage { get; private set; }
    public decimal OutputFrequency { get; private set; }
    public int LoadVA { get; private set; }
    public int LoadWatt
    {
        get => loadWatt;
        set
        {
            if (value != loadWatt)
            {
                loadWatt = value;
                double interval = (DateTime.Now - lastloadWattHourComputed).TotalSeconds;
                LoadWattHour = value / (3600 / Convert.ToDecimal(interval));
            }
        }
    }
    public decimal LoadWattHour
    {
        get => loadWattHour;
        set
        {
            if (value != loadWattHour && lastloadWattHourComputed != new DateTime())
            {
                loadWattHour = value;
            }

            lastloadWattHourComputed = DateTime.Now;
        }
    }
    public decimal LoadPercentage { get; private set; }
    public decimal BusVoltage { get; private set; }
    public decimal BatteryVoltage { get; private set; }
    public int BatteryChargeCurrent { get; private set; }
    public int BatteryCapacity { get; private set; }
    public int HeatSinkTemperature { get; private set; }
    public decimal PVInputCurrent { get; private set; }
    public decimal PVInputVoltage { get; private set; }
    public int PVInputWatt
    {
        get => pvInputWatt; set
        {
            if (value != pvInputWatt)
            {
                pvInputWatt = value;
                double interval = (DateTime.Now - lastpvInputWattHourComputed).TotalSeconds;
                PVInputWattHour = value / (3600 / Convert.ToDecimal(interval));
            }
        }
    }
    public decimal PVInputWattHour
    {
        get => pvInputWattHour; set
        {
            if (value != pvInputWattHour && lastpvInputWattHourComputed != new DateTime())
            {
                pvInputWattHour = value;
            }
            lastpvInputWattHourComputed = DateTime.Now;

        }
    }
    public decimal SCCVoltage { get; private set; }
    public int BatteryDischargeCurrent { get; private set; }
    public char PVOrACFeed { get; private set; }
    public char LoadOn { get; private set; }
    public char SCCOn { get; private set; }
    public char ACChargeOn { get; private set; }

    private int loadWatt;
    private decimal loadWattHour;
    private int pvInputWatt;
    private DateTime lastloadWattHourComputed;
    private decimal pvInputWattHour;
    private DateTime lastpvInputWattHourComputed;

    public bool Update()
    {
        if (ReadFailed)
            return false;

        string[]? parts = RawResponse[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        GridVoltage = decimal.Parse(parts[0]);
        GridFrequency = decimal.Parse(parts[1]);
        OutputVoltage = decimal.Parse(parts[2]);
        OutputFrequency = decimal.Parse(parts[3]);
        LoadVA = int.Parse(parts[4]);
        LoadWatt = int.Parse(parts[5]);
        LoadPercentage = decimal.Parse(parts[6]);
        BusVoltage = decimal.Parse(parts[7]);
        BatteryVoltage = decimal.Parse(parts[8]);
        BatteryChargeCurrent = int.Parse(parts[9]);
        BatteryCapacity = int.Parse(parts[10]);
        HeatSinkTemperature = int.Parse(parts[11]);
        PVInputCurrent = decimal.Parse(parts[12]);
        PVInputVoltage = decimal.Parse(parts[13]);
        SCCVoltage = decimal.Parse(parts[14]);
        BatteryDischargeCurrent = int.Parse(parts[15]);
        PVOrACFeed = parts[16][0];
        LoadOn = parts[16][3];
        SCCOn = parts[16][1];
        ACChargeOn = parts[16][2];
        PVInputWatt = int.Parse(parts[19]);

        return true;
    }
}
