using System.Text;

namespace InverterMon.Server.InverterService.Commands;

internal class BaseCommand
{
    public static FileStream Port { get; set; } = null!;
}

internal abstract class CommandBase<TResponseDto> : BaseCommand where TResponseDto : new()
{
    public TResponseDto Data { get; } = new();
    protected abstract string Command { get; }
    protected string RawResponse { private set; get; } = string.Empty;
    protected bool ReadFailed => !SendCommand();

    private bool SendCommand()
    {
        byte[]? cmdBytes = Encoding.ASCII.GetBytes(Command);
        ushort crc = CalculateCRC(cmdBytes);

        byte[]? buf = new byte[cmdBytes.Length + 3];
        Array.Copy(cmdBytes, buf, cmdBytes.Length);
        buf[cmdBytes.Length] = (byte)(crc >> 8);
        buf[cmdBytes.Length + 1] = (byte)(crc & 0xff);
        buf[cmdBytes.Length + 2] = 0x0d;

        Port.Write(buf, 0, buf.Length);
        byte[]? buffer = new byte[1024];
        int pos = 0;
        while (true)
        {
            try
            {
                int read = Port.Read(buffer, pos, buffer.Length - pos);

                if (read > 0)
                    pos += read;
                else
                    Thread.Sleep(5);

                if (buffer.Any(b => b == 0x0d))
                    break;
            }
            catch
            {
                Console.WriteLine("Connection lost!");
                _ = Inverter.Connect();
                return false;
            }
        }

        RawResponse =
            buffer.Any(b => b == 0x0d)
            ? Encoding.ASCII.GetString(buffer, 0, pos - 3)
            : string.Empty;

        return true;
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