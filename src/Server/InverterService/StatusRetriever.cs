using InverterMon.Server.Persistance;
using InverterMon.Server.Persistance.Settings;

namespace InverterMon.Server.InverterService;

class StatusRetriever : BackgroundService
{
    readonly CommandQueue _queue;
    readonly Database _db;
    readonly UserSettings _userSettings;

    public StatusRetriever(CommandQueue queue, Database db, UserSettings userSettings)
    {
        _queue = queue;
        _db = db;
        _userSettings = userSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        var cmd = _queue.StatusCommand;

        while (!c.IsCancellationRequested)
        {
            if (_queue.IsAcceptingCommands)
            {
                //feels hacky. find a better solution.
                cmd.Result.BatteryCapacity = _userSettings.BatteryCapacity;
                cmd.Result.PV_MaxCapacity = _userSettings.PV_MaxCapacity;

                _queue.AddCommands(cmd);
                _ = _db.UpdateTodaysPvGeneration(cmd, c);
            }
            await Task.Delay(Constants.StatusPollingFrequencyMillis);
        }
    }
}