using System.Text;
using System.Text.Json;

ushort[] crc_ta = { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef };
var port = File.Open("/dev/hidraw0", FileMode.Open, FileAccess.ReadWrite);

if (!port.CanWrite)
{
    Console.WriteLine("couldn't open port!!!");
    return;
}

var status = new Status();
var statusResult = SendCommand("QPIGS");
port.Close();
status.Parse(statusResult);
var statusStr = JsonSerializer.Serialize(status);
Console.WriteLine(statusStr);

string SendCommand(string cmd)
{
    var cmdBytes = Encoding.ASCII.GetBytes(cmd);
    var crc = CalculateCRC(cmdBytes);

    var buf = new byte[cmdBytes.Length + 3];
    Array.Copy(cmdBytes, buf, cmdBytes.Length);
    buf[cmdBytes.Length] = (byte)(crc >> 8);
    buf[cmdBytes.Length + 1] = (byte)(crc & 0xff);
    buf[cmdBytes.Length + 2] = 0x0d;

    port.Write(buf, 0, buf.Length);
    var buffer = new byte[1024];
    var pos = 0;
    while (true)
    {
        try
        {
            var read = port.Read(buffer, pos, buffer.Length - pos);
            if (read > 0)
            {
                pos += read;
            }
            else
            {
                Thread.Sleep(5);
            }
            if (buffer.Any(b => b == 0x0d)) { break; }
        }
        catch (TimeoutException)
        {
            break;
        }
    }

    if (buffer.Any(b => b == 0x0d))
    {
        var result = Encoding.ASCII.GetString(buffer, 0, pos - 3);
        //Console.WriteLine($"{cmd} Result (byte={pos}): {result}");
        return result;
    }

    return string.Empty;
}

ushort CalculateCRC(byte[] buffer)
{
    ushort crc = 0;
    var len = buffer.Length;

    foreach (var b in buffer)
    {
        var da = ((byte)(crc >> 8)) >> 4;
        crc <<= 4;
        crc ^= crc_ta[da ^ (b >> 4)];
        da = ((byte)(crc >> 8)) >> 4;
        crc <<= 4;
        crc ^= crc_ta[da ^ (b & 0x0F)];
    }

    var crcLow = (byte)crc;
    var crcHigh = (byte)(crc >> 8);
    if (crcLow is 0x28 or 0x0d or 0x0a) crcLow++;
    if (crcHigh is 0x28 or 0x0d or 0x0a) crcHigh++;

    crc = (ushort)(crcHigh << 8);
    crc += crcLow;

    return crc;
}