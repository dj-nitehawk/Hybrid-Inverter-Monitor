namespace USBDriver;

public class QPIGSCommand : Command
{
    private int loadWatt;
    private decimal loadWattHour;
    private int pvInputWatt;
    private DateTime lastloadWattHourComputed;
    private decimal pvInputWattHour;
    private DateTime lastpvInputWattHourComputed;

    public QPIGSCommand()
    {
        CommandName = "QPIGS";
        ResponseSize = 126;
    }

    public override void Parse(string rawData)
    {
        string[] dataElem = rawData.Substring(1).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine(rawData);
        /*int i = 0;

    foreach (var item in dataElem)
        {
            byte[] ba = Encoding.Default.GetBytes(item);
            var hexString = BitConverter.ToString(ba);
            Console.WriteLine($" {i++} => {item} {hexString}");
        }*/
        GridVoltage = Convert.ToDecimal(dataElem[0]);
        GridFrequency = Convert.ToDecimal(dataElem[1]);
        OutputVoltage = Convert.ToDecimal(dataElem[2]);
        OutputFrequency = Convert.ToDecimal(dataElem[3]);
        LoadVA = int.Parse(dataElem[4]);
        LoadWatt = int.Parse(dataElem[5]);

        LoadPercentage = Convert.ToDecimal(dataElem[6]);
        BusVoltage = Convert.ToDecimal(dataElem[7]);
        BatteryVoltage = Convert.ToDecimal(dataElem[8]);
        BatteryChargeCurrent = int.Parse(dataElem[9]);
        BatteryCapacity = int.Parse(dataElem[10]);
        HeatSinkTemperature = int.Parse(dataElem[11]);
        PVInputCurrent = Convert.ToDecimal(dataElem[12]);
        PVInputVoltage = Convert.ToDecimal(dataElem[13]);
        SCCVoltage = Convert.ToDecimal(dataElem[14]);
        BatteryDischargeCurrent = int.Parse(dataElem[15]);
        PVOrACFeed = dataElem[16][0];
        LoadOn = dataElem[16][3];
        SCCOn = dataElem[16][1];
        ACChargeOn = dataElem[16][2];
        PVInputWatt = int.Parse(dataElem[19]);
    }

    //("V", "power-plug")]
    public decimal GridVoltage { get; private set; }

    //("Hz", "current-ac")]
    public decimal GridFrequency { get; private set; }

    //"V", "power-plug")]
    public decimal OutputVoltage { get; private set; }

    //"Hz", "current-ac")]
    public decimal OutputFrequency { get; private set; }

    //"VA", "chart-bell-curve")]
    public int LoadVA { get; private set; }

    //("W", "chart-bell-curve")]
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

    //("Wh", "chart-bell-curve")]
    public decimal LoadWattHour
    {
        get => loadWattHour;
        set
        {
            if (value != loadWattHour && lastloadWattHourComputed != new DateTime())
                loadWattHour = value;
            lastloadWattHourComputed = DateTime.Now;
        }
    }

    //"%", "brightness-percent")]
    public decimal LoadPercentage { get; private set; }

    //"V", "details")]
    public decimal BusVoltage { get; private set; }

    //"V", "battery-outline")]
    public decimal BatteryVoltage { get; private set; }

    //"A", "current-dc")]
    public int BatteryChargeCurrent { get; private set; }

    //"%", "battery-outline")]
    public int BatteryCapacity { get; private set; }

    //"°C", "details")]
    public int HeatSinkTemperature { get; private set; }

    //"A", "solar-panel-large")]
    public decimal PVInputCurrent { get; private set; }

    //"V", "solar-panel-large")]
    public decimal PVInputVoltage { get; private set; }

    //"W", "solar-panel-large")]
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

    //("Wh", "solar-panel-large")]
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

    //"V", "current-dc")]
    public decimal SCCVoltage { get; private set; }

    //("A", "current-dc")]
    public int BatteryDischargeCurrent { get; private set; }

    //"", "power")]
    public char PVOrACFeed { get; private set; }

    //"", "power")]
    public char LoadOn { get; private set; }

    //"", "power")]
    public char SCCOn { get; private set; }

    //"", "power")]
    public char ACChargeOn { get; private set; }
}
