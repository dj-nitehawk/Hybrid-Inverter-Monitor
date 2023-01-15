using SerialPortLib;
using System.Globalization;

namespace InverterMon.Server.BatteryService;

public static class Extensions
{
    //this is the get all data command in hex format
    const string commandHex = "4E5700130000000006030000000000006800000129";

    public static void QueryData(this SerialPortInput port)
    {
        port.SendMessage(Convert.FromHexString(commandHex));
    }

    public static bool IsValid(this byte[] data)
    {
        if (data.Length < 8)
            return false;

        var header = Convert.ToHexString(data, 0, 2);

        if (header is not "4E57")
            return false;

        //sum up all bytes excluding the last 4 bytes
        var crc_calc = data[0..^3].Sum(Convert.ToInt32);

        //convert last 2 bytes to hex and get short from that hex
        var crc_lo = data[^2..^0].Read2Bytes(0);

        return crc_calc == crc_lo;
    }

    //note: the response is the byte representation of hex digits.
    //      i.e. the bytes cannot be converted to int without first converting to hex digits.

    public static ushort Read2Bytes(this byte[] input, ushort startPos)
    {
        var hex = Convert.ToHexString(input, startPos, 2);
        return ushort.Parse(hex, NumberStyles.HexNumber);
    }

    public static ushort Read4Bytes(this byte[] input, ushort startPos)
    {
        var hex = Convert.ToHexString(input, startPos, 4);
        return ushort.Parse(hex, NumberStyles.HexNumber);
    }
}