namespace InverterMon.Inverter;

internal static class Inverter
{
    public static Status Status { get; } = new();

    static Inverter()
    {
        try
        {
            CommandBase.Port = File.Open("/dev/hidraw0", FileMode.Open, FileAccess.ReadWrite);
        }
        catch
        {
            throw new Exception("Make sure the inverter is connected via USB port!!!");
        }
    }
}
