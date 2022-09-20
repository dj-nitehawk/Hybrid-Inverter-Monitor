using InverterMon.Server.InverterService.Commands;
using System.Text;

namespace InverterMon.Server.InverterService;

internal class CommandExecutor : BackgroundService
{
    private readonly CommandQueue queue;
    private FileStream? port;
    private readonly ILogger<CommandExecutor> log;
    private readonly IConfiguration confing;

    public CommandExecutor(CommandQueue queue, IConfiguration config, ILogger<CommandExecutor> log)
    {
        this.queue = queue;
        confing = config;
        this.log = log;
        Connect(new CancellationTokenSource(TimeSpan.FromHours(1)).Token).GetAwaiter().GetResult();
    }

    private async Task Connect(CancellationToken c)
    {
        var dev = confing["LaunchSettings:DeviceAddress"] ?? "/dev/hidraw0";

        while (!c.IsCancellationRequested)
        {
            try
            {
                port = File.Open(dev, FileMode.Open, FileAccess.ReadWrite);
                log.LogInformation("inverter connected!");
                break;
            }
            catch (Exception x)
            {
                log.LogError("inverter connection failed: {msg}", x.Message);
                await Task.Delay(5000, c);
            }
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
                    await ExecuteCommand(cmd, port!, c);
                    queue.IsAcceptingCommands = true;
                    queue.RemoveCommand();
                }
                catch (Exception x)
                {
                    queue.IsAcceptingCommands = false;
                    log.LogError("execution error: {msg}", x.Message);
                    await Task.Delay(5000);
                }
            }
            else
            {
                await Task.Delay(500, c);
            }
        }
    }

    private static async Task ExecuteCommand(ICommand command, FileStream port, CancellationToken c)
    {
        command.Start();
        byte[]? cmdBytes = Encoding.ASCII.GetBytes(command.CommandString);
        ushort crc = CalculateCRC(cmdBytes);

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

        command.Parse(Encoding.ASCII.GetString(buffer, 0, pos - 3));
        command.End();
    }

    private static readonly ushort[] crc_ta = { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef };

    private static ushort CalculateCRC(byte[] buffer)
    {
        ushort crc = 0;

        foreach (byte b in buffer)
        {
            int da = (byte)(crc >> 8) >> 4;
            crc <<= 4;
            crc ^= crc_ta[da ^ (b >> 4)];
            da = (byte)(crc >> 8) >> 4;
            crc <<= 4;
            crc ^= crc_ta[da ^ (b & 0x0F)];
        }

        byte crcLow = (byte)crc;
        byte crcHigh = (byte)(crc >> 8);
        if (crcLow is 0x28 or 0x0d or 0x0a)
        {
            crcLow++;
        }

        if (crcHigh is 0x28 or 0x0d or 0x0a)
        {
            crcHigh++;
        }

        crc = (ushort)(crcHigh << 8);
        crc += crcLow;

        return crc;
    }
}