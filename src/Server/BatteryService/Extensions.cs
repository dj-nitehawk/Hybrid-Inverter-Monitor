using SerialPortLib;
using System.Globalization;

namespace InverterMon.Server.BatteryService;

public static class Extensions
{
    const string commandHex = "4E5700130000000006030000000000006800000129";

    public static void QueryData(this SerialPortInput port)
    {
        port.SendMessage(Convert.FromHexString(commandHex));
    }

    public static ushort Read2Bytes(this byte[] input, ushort startPos)
    {
        if (startPos > input.Length - 2)
            return 0;

        var hex = Convert.ToHexString(input, startPos, 2);
        return ushort.Parse(hex, NumberStyles.HexNumber);
    }

    public static ushort Read4Bytes(this byte[] input, ushort startPos)
    {
        if (startPos > input.Length - 4)
            return 0;

        var hex = Convert.ToHexString(input, startPos, 4);
        return ushort.Parse(hex, NumberStyles.HexNumber);
    }
}