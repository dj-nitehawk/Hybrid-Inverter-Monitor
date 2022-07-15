using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Server.Persistance.PVGen;
using LiteDB;

namespace InverterMon.Server.Persistance;

public class Database
{
    private readonly LiteDatabase db;
    private readonly CommandQueue queue;
    private readonly ILiteCollection<PVGeneration> pvGenCollection;
    private PVGeneration? today;

    public Database(IHostApplicationLifetime lifetime, CommandQueue queue)
    {
        this.queue = queue;
        db = new("InverterMon.db") { CheckpointSize = 0 };
        pvGenCollection = db.GetCollection<PVGeneration>();
        lifetime.ApplicationStopping.Register(() => db?.Dispose());
        RestoreTodaysPVWattHours();
    }

    public void RestoreTodaysPVWattHours()
    {
        var todayDayNumer = DateOnly.FromDateTime(DateTime.Now).DayNumber;

        today = pvGenCollection
            .Query()
            .Where(pg => pg.Id == todayDayNumer)
            .SingleOrDefault();

        if (today is not null)
        {
            queue.StatusCommand.Result.RestorePVWattHours(today.TotalWattHours);
        }
        else
        {
            today = new PVGeneration { Id = todayDayNumer };
            today.SetTotalWattHours(0);
            queue.StatusCommand.Result.RestorePVWattHours(0);
            pvGenCollection.Insert(today);
            db.Checkpoint();
        }
    }

    public async Task UpdateTodaysPVGeneration(GetStatus cmd, CancellationToken c)
    {
        if (!cmd.ResultIsStale)
        {
            await cmd.WhileProcessing(c);

            var todayDayNumer = DateOnly.FromDateTime(DateTime.Now).DayNumber;

            if (today?.Id == todayDayNumer)
            {
                today.SetWattPeaks(cmd.Result.PVInputWatt);
                today.SetTotalWattHours(cmd.Result.PVInputWattHour);
                pvGenCollection.Update(today);
            }
            else
            {
                cmd.Result.ResetPVWattHourAccumulation(); //it's a new day. start accumulation from scratch.
                today = new PVGeneration { Id = todayDayNumer };
                today.SetTotalWattHours(0);
                pvGenCollection.Insert(today);
            }
            db.Checkpoint();
        }
    }

    public PVGeneration? GetPVGenForDay(int dayNumer)
        => pvGenCollection.FindOne(p => p.Id == dayNumer);
}