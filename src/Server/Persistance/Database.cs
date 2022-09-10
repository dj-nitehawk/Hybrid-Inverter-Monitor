using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Server.Persistance.PVGen;
using InverterMon.Server.Persistance.Settings;
using LiteDB;

namespace InverterMon.Server.Persistance;

public class Database
{
    private readonly LiteDatabase db;
    private readonly CommandQueue queue;
    private readonly UserSettings settings;
    private readonly ILiteCollection<PVGeneration> pvGenCollection;
    private readonly ILiteCollection<UserSettings> usrSettingsCollection;
    private PVGeneration? today;

    public Database(IHostApplicationLifetime lifetime, CommandQueue queue, UserSettings settings)
    {
        this.queue = queue;
        this.settings = settings;
        db = new("InverterMon.db") { CheckpointSize = 0 };
        lifetime.ApplicationStopping.Register(() => db?.Dispose());
        pvGenCollection = db.GetCollection<PVGeneration>();
        usrSettingsCollection = db.GetCollection<UserSettings>();
        RestoreTodaysPVWattHours();
        RestoreUserSettings();
    }

    //todo: break apart this class and put seperated logic in each vertical slice

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
        var hourNow = DateTime.Now.Hour;

        if (hourNow < settings.SunlightStartHour || hourNow >= settings.SunlightEndHour)
            return;

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

    public PVGeneration? GetPVGenForDay(int dayNumer)
        => pvGenCollection.FindOne(p => p.Id == dayNumer);

    public void RestoreUserSettings()
    {
        var settings = usrSettingsCollection.FindById(1);
        if (settings is not null)
        {
            this.settings.PVMaxCapacity = settings.PVMaxCapacity;
            this.settings.BatteryCapacity = settings.BatteryCapacity;
            this.settings.SunlightStartHour = settings.SunlightStartHour;
            this.settings.SunlightEndHour = settings.SunlightEndHour;
        }
        else
        {
            usrSettingsCollection.Insert(this.settings);
            db.Checkpoint();
        }
    }

    public void UpdateUserSettings(UserSettings settings)
    {
        usrSettingsCollection.Update(settings);
        db.Checkpoint();
    }
}