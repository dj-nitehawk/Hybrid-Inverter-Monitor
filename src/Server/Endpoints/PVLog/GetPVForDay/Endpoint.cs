using InverterMon.Server.Persistance;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.PVLog.GetPVForDay;

public class Endpoint : Endpoint<Request, PVDay>
{
    public Database Db { get; set; }

    public override void Configure()
    {
        Get("/pv-log/get-pv-for-day/{DayNumber}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        var pvDay = Db.GetPVGenForDay(r.DayNumber);

        if (pvDay is null)
        {
            await SendNotFoundAsync();
            return;
        }

        if (Env.IsDevelopment() && pvDay.TotalWattHours == 0)
        {
            pvDay = new()
            {
                Id = DateOnly.FromDateTime(DateTime.Now).DayNumber,
                TotalWattHours = Random.Shared.Next(3000),
            };

            //pvDay.AllocateBuckets(6, 18);

            for (int i = 0; i < 97; i++)
                pvDay.WattPeaks.Add(i.ToString(), Random.Shared.Next(2000));
        }

        Response.TotalKiloWattHours = Math.Round(pvDay.TotalWattHours / 1000, 2);
        Response.DayNumber = pvDay.Id;
        Response.DayName = DateOnly.FromDayNumber(pvDay.Id).ToString("dddd MMMM dd");
        Response.WattPeaks = pvDay.WattPeaks.Select(p => new PVDay.WattPeak
        {
            MinuteBucket = p.Key,
            PeakWatt = p.Value
        });
    }
}