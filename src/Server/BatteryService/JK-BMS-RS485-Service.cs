using InverterMon.Shared.Models;
using SerialPortLib;

namespace InverterMon.Server.BatteryService;

public class JkBms
{
    public BMSStatus Status { get; } = new();
    public bool IsConnected => Status.PackVoltage > 0;

    private readonly int pollFrequencyMillis = 1000;
    private readonly AmpValQueue recentAmpReadings = new(10); //avg value over 10 readings (~10secs)
    private readonly SerialPortInput bms = new();

    public JkBms(IConfiguration config, ILogger<JkBms> logger, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            FillDummyData();
            return;
        }

        var bmsAddress = config["LaunchSettings:JkBmsAddress"] ?? "/dev/ttyUSB0";
        bms.SetPort(bmsAddress, 115200);
        bms.ConnectionStatusChanged += ConnectionStatusChanged;
        bms.MessageReceived += MessageReceived;

        Task.Run(async () =>
        {
            var ct = new CancellationTokenSource(TimeSpan.FromHours(1)).Token;
            while (!ct.IsCancellationRequested && !bms.IsConnected)
            {
                var success = bms.Connect();
                if (success)
                {
                    logger.LogInformation("bms port opened!");
                }
                else
                {
                    logger.LogWarning("trying to open bms port at: {address}", bmsAddress);
                    await Task.Delay(10000);
                }
            }
        });
    }

    private void ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
    {
        if (e.Connected)
            bms.QueryData();
    }

    private async void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        var response = e.Data[11..]; //skip the first 10 bytes
        var cellCount = response[1] / 3; //pos 1 is total cell bytes length. 3 bytes per cell.
        if (cellCount is 0 or > 24) return; //todo: replace this with crc check

        ushort pos = 3;
        for (byte i = 1; pos <= response.Length - 2 && i <= cellCount; i++)
        {
            //cell voltage groups (of 3 bytes) start at pos 2
            //first cell voltage starts at position 3 (pos 2 is cell number). voltage value is next 2 bytes.
            // ex: .....,1,X,X,2,Y,Y,3,Z,Z
            Status.Cells[i] = response.Read2Bytes(pos) / 1000f;
            if (i < cellCount)
                pos += 3;
        }

        //position is increased by 3 bytes in order to skip the address/code byte
        pos += 3;
        Status.MosTemp = response.Read2Bytes(pos);
        pos += 3;
        Status.Temp1 = response.Read2Bytes(pos);
        pos += 3;
        Status.Temp2 = response.Read2Bytes(pos);

        pos += 3;
        Status.PackVoltage = response.Read2Bytes(pos) / 100f;

        pos += 3;
        var rawVal = response.Read2Bytes(pos);
        Status.IsCharging = Convert.ToBoolean(int.Parse(Convert.ToString(rawVal, 2).PadLeft(16, '0')[..1])); //pick first bit of padded 16 bit binary representation and turn it in to a bool

        rawVal &= (1 << 15) - 1; //unset the MSB with a bitmask
        var ampVal = rawVal / 100f;
        recentAmpReadings.Enqueue(ampVal);
        Status.AvgCurrentAmps = recentAmpReadings.GetAverage();

        pos += 3;
        Status.CapacityPct = Convert.ToUInt16(response[pos]);

        pos += 103;
        Status.PackCapacity = response.Read4Bytes(pos);

        var timeLeft = 0f;

        if (Status.AvgCurrentAmps > 0)
        {
            if (Status.IsCharging)
                timeLeft = (Status.PackCapacity - Status.AvailableCapacity) / Status.AvgCurrentAmps;
            else
                timeLeft = Status.AvailableCapacity / Status.AvgCurrentAmps;

            var tSpan = TimeSpan.FromHours(timeLeft);
            Status.TimeHrs = (ushort)tSpan.TotalHours;
            Status.TimeMins = tSpan.Minutes;
        }
        else
        {
            Status.TimeHrs = 0;
            Status.TimeMins = 0;
        }

        await Task.Delay(pollFrequencyMillis);
        bms.QueryData();
    }

    private void FillDummyData()
    {
        Status.MosTemp = 30;
        Status.Temp1 = 28;
        Status.Temp2 = 29;
        Status.PackVoltage = 25.6f;
        Status.IsCharging = true;
        Status.AvgCurrentAmps = 20;
        Status.CapacityPct = 50;
        Status.PackCapacity = 120;
        Status.TimeHrs = 3;
        Status.TimeMins = 11;
        for (byte i = 1; i <= 8; i++)
            Status.Cells.Add(i, 1.111f);
    }
}
