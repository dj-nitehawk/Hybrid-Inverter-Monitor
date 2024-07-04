namespace InverterMon.Server.Persistance.PVGen;

public static class PVGenExtensions
{
    public static TimeSpan BucketDuration => TimeSpan.FromMinutes(5);
    static readonly string bucketKey = "HH:mm";

    public static string ToTimeBucket(this DateTime dt)
    {
        var delta = dt.Ticks % BucketDuration.Ticks;
        return new DateTime(dt.Ticks - delta, dt.Kind).ToString(bucketKey);
    }

    public static void AllocateBuckets(this PVGeneration pvGen, int startHour, int endHour)
    {
        var timeOfDay = new TimeOnly(startHour, 0);

        while (timeOfDay.Hour < endHour)
        {
            pvGen.WattPeaks[timeOfDay.ToString(bucketKey)] = 0;
            timeOfDay = timeOfDay.AddMinutes(BucketDuration.TotalMinutes);
        }
    }
}