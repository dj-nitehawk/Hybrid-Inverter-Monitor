using HidSharp;
using System.Text;
using ICommand = InverterMon.Server.InverterService.Commands.ICommand;

namespace InverterMon.Server.InverterService;

internal class CommandExecutor : BackgroundService
{
    private readonly CommandQueue queue;
    private DeviceStream? dev;
    private readonly ILogger<CommandExecutor> log;
    private readonly IConfiguration confing;

    public CommandExecutor(CommandQueue queue, IConfiguration config, ILogger<CommandExecutor> log)
    {
        this.queue = queue;
        confing = config;
        this.log = log;

        log.LogInformation("connecting to the inverter...");

        while (!Connect())
            Thread.Sleep(10000);
    }

    private bool Connect()
    {
        var devPath = confing["LaunchSettings:DeviceAddress"] ?? "/dev/hidraw0";

        dev = DeviceList.Local
            .GetDevices(
               types: DeviceTypes.Hid | DeviceTypes.Serial,
               filter: d => DeviceFilterHelper.MatchHidDevices(d, 0x0665, 0x5161) || DeviceFilterHelper.MatchSerialDevices(d, devPath))
            .FirstOrDefault()?.Open();

        if (dev is null)
        {
            return false;
        }
        else
        {
            log.LogInformation("inverter connected!");
            return true;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        while (!c.IsCancellationRequested)
        {
            var cmd = queue.GetCommand();
            if (cmd is not null)
            {
                try
                {
                    await ExecuteCommand(cmd, dev!, c);
                    queue.IsAcceptingCommands = true;
                    queue.RemoveCommand();
                }
                catch (Exception x)
                {
                    queue.IsAcceptingCommands = false;
                    log.LogError("command error: {msg}", x.Message);
                    dev!.Close();
                    dev.Dispose();
                    log.LogInformation("exiting...");
                    Environment.Exit(0);
                }
            }
            else
            {
                await Task.Delay(500, c);
            }
        }
    }

    private static async Task ExecuteCommand(ICommand command, Stream port, CancellationToken c)
    {
        command.Start();
        byte[]? cmdBytes = Encoding.ASCII.GetBytes(command.CommandString);
        ushort crc = CalculateXmodemCrc16(command.CommandString);

        byte[]? buf = new byte[cmdBytes.Length + 3];
        Array.Copy(cmdBytes, buf, cmdBytes.Length);
        buf[cmdBytes.Length] = (byte)(crc >> 8);
        buf[cmdBytes.Length + 1] = (byte)(crc & 0xff);
        buf[cmdBytes.Length + 2] = 0x0d;

        await port.WriteAsync(buf, c);
        byte[]? buffer = new byte[1024];
        int pos = 0;
        do
        {
            int readCount = await port.ReadAsync(buffer.AsMemory(pos, buffer.Length - pos), c);
            if (readCount > 0)
                pos += readCount;
        }
        while (!buffer.Any(b => b == 0x0d));

        command.Parse(Encoding.ASCII.GetString(buffer, 0, pos - 3).Sanitize());
        command.End();
    }

    private static ushort CalculateXmodemCrc16(string data)
    {
        ushort crc = 0;
        for (int i = 0; i < data.Length; i++)
        {
            crc ^= (ushort)(data[i] << 8);
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 0x8000) != 0)
                    crc = (ushort)((crc << 1) ^ 0x1021);
                else
                    crc <<= 1;
            }
        }
        return crc;
    }
}