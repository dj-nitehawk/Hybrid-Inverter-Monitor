using InverterMon.Server.InverterService.Commands;

namespace InverterMon.Server.InverterService;

internal static class Inverter
{
    public static bool IsConnected { get; set; }
    public static Status Status { get; } = new();

    static Inverter()
        => Connect();

    public static bool Connect()
    {
        try
        {
            OpenPort();
        }
        catch
        {
            IsConnected = false;
            lock (CommandBase.Port ??= new FileStream("/dev/null", FileMode.Open))
            {
                CommandBase.Port.Dispose();
                while (true)
                {
                    Thread.Sleep(10000);
                    try
                    {
                        OpenPort();
                        if (IsConnected)
                            break;
                    }
                    catch
                    {
                        Console.WriteLine("Inverter is not connected :-(");
                    }
                }
            }
        }
        return true;
    }

    private static void OpenPort()
    {
        Console.WriteLine("Connecting to inverter...");
        CommandBase.Port = File.Open("/dev/hidraw0", FileMode.Open, FileAccess.ReadWrite);
        IsConnected = true;
    }
}
