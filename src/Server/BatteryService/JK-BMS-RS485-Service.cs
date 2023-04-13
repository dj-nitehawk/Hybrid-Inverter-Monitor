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

    public JkBms(IConfiguration config, ILogger<JkBms> logger, IWebHostEnvironment env, IHostApplicationLifetime applife)
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
        applife.ApplicationStopping.Register(bms.Disconnect);

        Task.Run(async () =>
        {
            var ct = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;
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

    private void MessageReceived(object sender, MessageReceivedEventArgs evnt)
    {
        var data = evnt.Data.AsSpan();

        if (!data.IsValid())
        {
            Thread.Sleep(pollFrequencyMillis);
            bms.QueryData();
            return;
        }

        var res = data[11..]; //skip the first 10 bytes
        var cellCount = res[1] / 3; //pos 1 is total cell bytes length. 3 bytes per cell.

        ushort pos = 0;
        for (byte i = 1; i <= cellCount; i++)
        {
            //cell voltage groups (of 3 bytes) start at pos 2
            //first cell voltage starts at position 3 (pos 2 is cell number). voltage value is next 2 bytes.
            // ex: .....,1,X,X,2,Y,Y,3,Z,Z
            //position is increased by 3 bytes in order to skip the address/code byte
            Status.Cells[i] = res.Read2Bytes(pos += 3) / 1000f;
        }

        Status.MosTemp = res.Read2Bytes(pos += 3);
        Status.Temp1 = res.Read2Bytes(pos += 3);
        Status.Temp2 = res.Read2Bytes(pos += 3);
        Status.PackVoltage = res.Read2Bytes(pos += 3) / 100f;

        var rawVal = res.Read2Bytes(pos += 3);
        Status.IsCharging = Convert.ToBoolean(int.Parse(Convert.ToString(rawVal, 2).PadLeft(16, '0')[..1])); //pick first bit of padded 16 bit binary representation and turn it in to a bool

        rawVal &= (1 << 15) - 1; //unset the MSB with a bitmask to get correct ampere reading
        var ampVal = rawVal / 100f;
        recentAmpReadings.Store(ampVal, Status.IsCharging);

        Status.AvgCurrentAmps = recentAmpReadings.GetAverage();
        Status.CapacityPct = Convert.ToUInt16(res[pos += 3]);
        Status.IsWarning = res.Read2Bytes(pos += 15) > 0;
        Status.PackCapacity = res.Read4Bytes(pos += 88);

        if (Status.AvgCurrentAmps > 0)
        {
            float timeLeft;
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

        Thread.Sleep(pollFrequencyMillis);
        bms.QueryData();
    }

    private void FillDummyData()
    {
        Status.MosTemp = 30;
        Status.Temp1 = 28;
        Status.Temp2 = 29;
        Status.PackVoltage = 25.6f;
        Status.IsCharging = true;
        Status.AvgCurrentAmps = 21.444f;
        Status.CapacityPct = 50;
        Status.PackCapacity = 120;
        Status.IsWarning = false;
        Status.TimeHrs = 3;
        Status.TimeMins = 11;
        for (byte i = 1; i <= 8; i++)
            Status.Cells.Add(i, 1.110f);
    }
}
