﻿using System.IO.Ports;
using System.Text;

namespace InverterMon.Server.InverterService;

public static class Inverter
{
    static SerialPort? _serialPort;
    static FileStream? _fileStream;

    public static bool Connect(string devicePath, ILogger logger)
    {
        try
        {
            if (devicePath.Contains("/hidraw", StringComparison.OrdinalIgnoreCase))
            {
                _fileStream = new(devicePath, FileMode.Open, FileAccess.ReadWrite);

                return true;
            }

            if (devicePath.Contains("/ttyUSB", StringComparison.OrdinalIgnoreCase) || devicePath.Contains("COM", StringComparison.OrdinalIgnoreCase))
            {
                _serialPort = new(devicePath)
                {
                    BaudRate = 2400,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None
                };
                _serialPort.Open();

                return true;
            }
            logger.LogError("device path [{path}] is not acceptable!", devicePath);
        }
        catch (Exception x)
        {
            logger.LogError("connection error at [{path}]. reason: [{reason}]", devicePath, x.Message);
        }

        return false;
    }

    public static void Disconnect()
    {
        _serialPort?.Close();
        _serialPort?.Dispose();
        _fileStream?.Close();
        _fileStream?.Dispose();
    }

    static readonly byte[] _writeBuffer = new byte[512];

    public static Task Write(string command, CancellationToken ct)
    {
        var cmdBytes = Encoding.ASCII.GetBytes(command);
        var crc = CalculateXmodemCrc16(command);

        Buffer.BlockCopy(cmdBytes, 0, _writeBuffer, 0, cmdBytes.Length);
        _writeBuffer[cmdBytes.Length] = (byte)(crc >> 8);
        _writeBuffer[cmdBytes.Length + 1] = (byte)(crc & 0xff);
        _writeBuffer[cmdBytes.Length + 2] = 0x0d;

        if (_fileStream != null)
            return _fileStream.WriteAsync(_writeBuffer, 0, cmdBytes.Length + 3, ct);

        return _serialPort != null
                   ? _serialPort.BaseStream.WriteAsync(_writeBuffer, 0, cmdBytes.Length + 3, ct)
                   : Task.CompletedTask;
    }

    static readonly byte[] _readBuffer = new byte[1024];

    public static async Task<string> Read(CancellationToken ct)
    {
        var pos = 0;
        const byte eol = 0x0d;

        if (_fileStream != null)
        {
            do
            {
                var readCount = await _fileStream.ReadAsync(_readBuffer.AsMemory(pos, _readBuffer.Length - pos), ct);

                if (readCount > 0)
                {
                    pos += readCount;

                    for (var i = pos - readCount; i < pos; i++)
                    {
                        if (_readBuffer[i] == eol)
                            return Encoding.ASCII.GetString(_readBuffer, 0, i - 2).Sanitize();
                    }
                }
            } while (pos < _readBuffer.Length);
        }
        else if (_serialPort != null)
        {
            do
            {
                var readCount = await _serialPort.BaseStream.ReadAsync(_readBuffer.AsMemory(pos, _readBuffer.Length - pos), ct);

                if (readCount > 0)
                {
                    pos += readCount;

                    for (var i = pos - readCount; i < pos; i++)
                    {
                        if (_readBuffer[i] == eol)
                            return Encoding.ASCII.GetString(_readBuffer, 0, i - 2).Sanitize();
                    }
                }
            } while (pos < _readBuffer.Length);
        }
        else
            throw new InvalidOperationException("inverter not connected.");

        throw new InvalidOperationException("buffer overflow.");
    }

    static ushort CalculateXmodemCrc16(string data)
    {
        ushort crc = 0;
        var length = data.Length;

        for (var i = 0; i < length; i++)
        {
            crc ^= (ushort)(data[i] << 8);

            for (var j = 0; j < 8; j++)
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