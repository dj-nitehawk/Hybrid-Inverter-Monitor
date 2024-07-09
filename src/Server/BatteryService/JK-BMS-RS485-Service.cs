using InverterMon.Server.Persistance.Settings;
using InverterMon.Shared.Models;
using SerialPortLib;

namespace InverterMon.Server.BatteryService;

public class JkBms
{
    public BMSStatus Status { get; } = new();
    public bool IsConnected => Status.PackVoltage > 0;

    const int PollFrequencyMillis = 1000;
    readonly AmpValQueue _recentAmpReadings = new(5); //avg value over 5 readings (~5secs)
    readonly SerialPortInput _bms = new();

    public JkBms(UserSettings userSettings, IConfiguration config, ILogger<JkBms> logger, IWebHostEnvironment env, IHostApplicationLifetime appLife)
    {
        if (env.IsDevelopment())
        {
            FillDummyData();

            return;
        }

        Status.PackNominalVoltage = userSettings.BatteryNominalVoltage;
        var bmsAddress = config["LaunchSettings:JkBmsAddress"] ?? "/dev/ttyUSB0";
        _bms.SetPort(bmsAddress);
        _bms.ConnectionStatusChanged += ConnectionStatusChanged;
        _bms.MessageReceived += MessageReceived;
        appLife.ApplicationStopping.Register(_bms.Disconnect);

        Task.Run(
            async () =>
            {
                var ct = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

                while (!ct.IsCancellationRequested && !_bms.IsConnected)
                {
                    var success = _bms.Connect();

                    if (success)
                        logger.LogInformation("bms port opened!");
                    else
                    {
                        logger.LogWarning("trying to open bms port at: {address}", bmsAddress);
                        await Task.Delay(10000);
                    }
                }
            });
    }

    void ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
    {
        if (e.Connected)
            _bms.QueryData();
    }

    void MessageReceived(object sender, MessageReceivedEventArgs evnt)
    {
        var data = evnt.Data.AsSpan();

        if (!data.IsValid())
        {
            Thread.Sleep(PollFrequencyMillis);
            _bms.QueryData();

            return;
        }

        var res = data[11..];       //skip the first 10 bytes
        var cellCount = res[1] / 3; //pos 1 is total cell bytes length. 3 bytes per cell.

        ushort pos = 0;
        for (byte i = 1; i <= cellCount; i++)

            //cell voltage groups (of 3 bytes) start at pos 2
            //first cell voltage starts at position 3 (pos 2 is cell number). voltage value is next 2 bytes.
            // ex: .....,1,X,X,2,Y,Y,3,Z,Z
            //position is increased by 3 bytes in order to skip the address/code byte
            Status.Cells[i] = res.Read2Bytes(pos += 3) / 1000f;

        Status.MosTemp = res.Read2Bytes(pos += 3);
        Status.Temp1 = res.Read2Bytes(pos += 3);
        Status.Temp2 = res.Read2Bytes(pos += 3);
        Status.PackVoltage = res.Read2Bytes(pos += 3) / 100f;

        var rawVal = res.Read2Bytes(pos += 3);
        Status.IsCharging =
            Convert.ToBoolean(
                int.Parse(Convert.ToString(rawVal, 2).PadLeft(16, '0')[..1])); //pick first bit of padded 16 bit binary representation and turn it in to a bool

        rawVal &= (1 << 15) - 1; //unset the MSB with a bitmask to get correct ampere reading
        var ampVal = rawVal / 100f;
        _recentAmpReadings.Store(ampVal, Status.IsCharging);

        Status.AvgCurrentAmps = _recentAmpReadings.GetAverage();
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

        Thread.Sleep(PollFrequencyMillis);
        _bms.QueryData();
    }

    void FillDummyData()
    {
        Status.MosTemp = 30.1f;
        Status.Temp1 = 28.5f;
        Status.Temp2 = 29.2f;
        Status.PackVoltage = 25.6f;
        Status.IsCharging = true;
        Status.AvgCurrentAmps = 21.444f;
        Status.CapacityPct = 90;
        Status.PackCapacity = 320;
        Status.PackNominalVoltage = 51.2f;
        Status.IsWarning = false;
        Status.TimeHrs = 24;
        Status.TimeMins = 10;
        for (byte i = 1; i <= 8; i++)
            Status.Cells.Add(i, 1.110f);
    }
}