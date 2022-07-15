namespace InverterMon.Server.Persistance.PVGen;

public static class PVGenExtensions
{
    private static readonly TimeSpan d = TimeSpan.FromMinutes(1);

    public static string ToKey(this DateTime dt)
    {
        var delta = dt.Ticks % d.Ticks;
        return new DateTime(dt.Ticks - delta, dt.Kind).ToString("HH:mm");
    }
}