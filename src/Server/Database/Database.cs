using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Server.Logging;
using LiteDB;

namespace InverterMon.Server.Database;

public class Database
{
    private readonly LiteDatabase db;
    private readonly CommandQueue queue;
    private readonly ILiteCollection<PVGeneration> pvGenCollection;

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
        var today = pvGenCollection
            .Query()
            .Where(pg => pg.Id == DateOnly.FromDateTime(DateTime.Today).DayNumber)
            .SingleOrDefault();

        if (today is not null)
        {
            queue.StatusCommand.Result.RestorePVWattHours(today.TotalWattHours);
        }
        else
        {
            pvGenCollection.Insert(new PVGeneration { Id = DateOnly.FromDateTime(DateTime.Today).DayNumber });
            db.Checkpoint();
        }
    }

    public async Task UpdateTodaysPVGeneration(GetStatus cmd, CancellationToken c)
    {
        if (!cmd.ResultIsStale)
        {
            await cmd.WhileProcessing(c);

            var today = pvGenCollection
                .Query()
                .Where(pg => pg.Id == DateOnly.FromDateTime(DateTime.Today).DayNumber)
                .SingleOrDefault();

            if (today is not null)
            {
                today.SetHourlyMaximum(cmd.Result.PVInputWatt);
                today.SetTotalWattHours(cmd.Result.PVInputWattHour);
                pvGenCollection.Update(today);
            }
            else
            {
                cmd.Result.PVInputWattHour = 0; //it's a new day. start accumulation from scratch.
                pvGenCollection.Insert(new PVGeneration
                {
                    Id = DateOnly.FromDateTime(DateTime.Today).DayNumber,
                });
            }
            db.Checkpoint();
        }
    }
}