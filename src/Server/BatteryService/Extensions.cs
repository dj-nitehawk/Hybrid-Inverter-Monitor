﻿using SerialPortLib;
using System.Globalization;

namespace InverterMon.Server.BatteryService;

public static class Extensions
{
    //this is the get all data command
    static readonly byte[] _cmd = Convert.FromHexString("4E5700130000000006030000000000006800000129");

    public static void QueryData(this SerialPortInput port)
    {
        port.SendMessage(_cmd);
    }

    //note: the response is the byte representation of hex digits.
    //      i.e. the bytes cannot be converted to int/short without first converting to hex digits.

    public static ushort Read2Bytes(this Span<byte> data, ushort startPos)
    {
        var hex = Convert.ToHexString(data.Slice(startPos, 2));

        return ushort.Parse(hex, NumberStyles.HexNumber);
    }

    public static uint Read4Bytes(this Span<byte> input, ushort startPos)
    {
        var hex = Convert.ToHexString(input.Slice(startPos, 4));

        return uint.Parse(hex, NumberStyles.HexNumber);
    }

    public static bool IsValid(this Span<byte> data)
    {
        if (data.Length < 8)
            return false;

        var header = Convert.ToHexString(data[..2]); //get hex from first 2 bytes

        if (header is not "4E57")
            return false;

        short crcCalc = 0;

        foreach (var b in data[..^3]) //sum up all bytes excluding the last 4 bytes
            crcCalc += b;

        //convert last 2 bytes to hex and get short from that hex
        var crcLo = data[^2..].Read2Bytes(0);

        return crcCalc == crcLo;
    }
}