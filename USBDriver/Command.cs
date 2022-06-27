using System.Text;

namespace USBDriver;

public abstract class Command
{
    public string CommandName { get; set; } = "QPIGS";
    public int ResponseSize { get; set; }

    public abstract void Parse(string rawData);

    public virtual string ReadCommand(Stream hidStream)
    {
        byte[] cmd = Encoding.ASCII.GetBytes(CommandName);
        byte[] crc = BitConverter.GetBytes(GenCrc16(cmd));
        byte[] toWrite = new byte[cmd.Length + crc.Length + 1];
        Buffer.BlockCopy(cmd, 0, toWrite, 0, cmd.Length);
        Buffer.BlockCopy(crc, 0, toWrite, cmd.Length, crc.Length);
        toWrite[toWrite.Length - 1] = 0x0d;
        //Console.WriteLine(System.Text.Encoding.ASCII.GetString(toWrite));
        hidStream.Write(toWrite, 0, toWrite.Length);
        byte[] buf = new byte[Math.Min(9, ResponseSize)];
        byte[] resBuffer = new byte[ResponseSize];
        int totalRead = 0;
        while (totalRead < ResponseSize)
        {
            int read = hidStream.Read(buf, 0, buf.Length);
            //  Console.WriteLine($"Loop TotalRead={totalRead}, Count={read}");
            if (totalRead + read > ResponseSize)
                Buffer.BlockCopy(buf, 0, resBuffer, totalRead, ResponseSize - totalRead);
            else
                Buffer.BlockCopy(buf, 0, resBuffer, totalRead, read);
            totalRead += read;
        }
        //Console.WriteLine($"TotalRead={totalRead}, Count={read}");
        //System.Buffer.BlockCopy(buf, 0, resBuffer, totalRead, read);

        return Encoding.ASCII.GetString(resBuffer).Replace("\0", "");
    }

    public virtual void ProcessCommand(Stream hidStream)
        => Parse(ReadCommand(hidStream));

    public static ushort GenCrc16(byte[] c)
    {
        const ushort Polynominal = 0x1021;
        const ushort InitValue = 0x0;
        ushort i, j, index = 0;
        ushort CRC = InitValue;
        ushort Remainder, tmp, short_c;
        for (i = 0; i < c.Length; i++)
        {
            short_c = (ushort)(0x00ff & c[index]);
            tmp = (ushort)((CRC >> 8) ^ short_c);
            Remainder = (ushort)(tmp << 8);
            for (j = 0; j < 8; j++)
                Remainder = (Remainder & 0x8000) != 0 ? (ushort)((Remainder << 1) ^ Polynominal) : (ushort)(Remainder << 1);
            CRC = (ushort)((CRC << 8) ^ Remainder);
            index++;
        }
        return CRC;
    }
}