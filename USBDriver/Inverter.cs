using HidSharp;

namespace USBDriver;

public static class Inverter
{
    private static HidStream? _device;

    public static bool Connect()
    {
        return _device is null &&
            DeviceList.Local.GetHidDevices().FirstOrDefault(d => d.DevicePath.Contains("0665"))?.TryOpen(out _device) is true;
    }

    public static void DisableTimeOut()
    {
        ThrowIfNotConnected();
        _device!.ReadTimeout = Timeout.Infinite;
    }

    public static void Disconnect()
    {
        _device?.Close();
        _device?.Dispose();
        _device = null;
    }

    private static void ThrowIfNotConnected()
    {
        if (_device is null)
            throw new InvalidOperationException("Inverter is not connected!");
    }
}
