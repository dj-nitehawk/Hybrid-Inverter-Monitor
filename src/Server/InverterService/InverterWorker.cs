using InverterMon.Server.InverterService.Commands;
using System.Text;

namespace InverterMon.Server.InverterService;

internal class InverterWorker : BackgroundService
{
    private readonly CommandQueue queue;
    private FileStream? port;
    private readonly ILogger<InverterWorker> log;

    public InverterWorker(CommandQueue queue, ILogger<InverterWorker> log)
    {
        this.queue = queue;
        this.log = log;
        Connect(new CancellationTokenSource(TimeSpan.FromHours(1)).Token).GetAwaiter().GetResult();
    }

    private async Task Connect(CancellationToken c)
    {
        while (!c.IsCancellationRequested)
        {
            try
            {
                if (port is not null)
                {
                    await port.DisposeAsync();
                    port = null;
                }
                port = File.Open("/dev/hidraw0", FileMode.Open, FileAccess.ReadWrite);
                log.LogInformation("connected to inverter!");
                break;
            }
            catch (Exception x)
            {
                log.LogError("connect error: {msg}", x.Message);
                await Task.Delay(5000, c);
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        while (!c.IsCancellationRequested)
        {
            if (queue.Commands.TryDequeue(out ICommand? cmd))
            {
                try
                {
                    await ExecuteCommand(cmd, c);
                }
                catch (Exception x)
                {
                    log.LogError("execution error: {msg}", x.Message);
                    await Connect(c);
                }
            }
            await Task.Delay(500, c);
        }
    }

    private async Task ExecuteCommand(ICommand command, CancellationToken c)
    {
        byte[]? cmdBytes = Encoding.ASCII.GetBytes(command.CommandString);
        ushort crc = CalculateCRC(cmdBytes);

        byte[]? buf = new byte[cmdBytes.Length + 3];
        Array.Copy(cmdBytes, buf, cmdBytes.Length);
        buf[cmdBytes.Length] = (byte)(crc >> 8);
        buf[cmdBytes.Length + 1] = (byte)(crc & 0xff);
        buf[cmdBytes.Length + 2] = 0x0d;

        await port!.WriteAsync(buf, c);
        byte[]? buffer = new byte[1024];
        int pos = 0;
        do
        {
            int read = await port!.ReadAsync(buffer.AsMemory(pos, buffer.Length - pos), c);
            if (read > 0)
                pos += read;
            else
                Thread.Sleep(5);
        }
        while (!buffer.Any(b => b == 0x0d));

        command.Parse(Encoding.ASCII.GetString(buffer, 0, pos - 3));
        command.IsComplete = true;

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