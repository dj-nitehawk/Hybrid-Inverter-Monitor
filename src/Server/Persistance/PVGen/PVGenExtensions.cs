namespace InverterMon.Server.Persistance.PVGen;

public static class PVGenExtensions
{
    private static readonly TimeSpan bucketDuration = TimeSpan.FromMinutes(5);
    private static readonly string bucketKey = "HH:mm";

    public static string ToTimeBucket(this DateTime dt)
    {
        var delta = dt.Ticks % bucketDuration.Ticks;
        return new DateTime(dt.Ticks - delta, dt.Kind).ToString(bucketKey);
    }

    public static void AllocateBuckets(this PVGeneration pvGen, int startHour, int endHour)
    {
        var timeOfDay = new TimeOnly(startHour, 0);

        while (timeOfDay.Hour < endHour)
        {
            pvGen.WattPeaks[timeOfDay.ToString(bucketKey)] = 0;
            timeOfDay = timeOfDay.AddMinutes(bucketDuration.TotalMinutes);
        }
    }
}