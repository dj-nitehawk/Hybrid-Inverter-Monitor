using InverterMon.Server.Persistance;
using InverterMon.Server.Persistance.Settings;

namespace InverterMon.Server.InverterService;

class StatusRetriever(CommandQueue queue, Database db, UserSettings userSettings) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken c)
    {
        var cmd = queue.StatusCommand;

        while (!c.IsCancellationRequested)
        {
            if (queue.IsAcceptingCommands)
            {
                //feels hacky. find a better solution.
                cmd.Result.BatteryCapacity = userSettings.BatteryCapacity;
                cmd.Result.PV_MaxCapacity = userSettings.PV_MaxCapacity;

                queue.AddCommands(cmd);
                _ = db.UpdateTodaysPvGeneration(cmd, c);
            }
            await Task.Delay(Constants.StatusPollingFrequencyMillis);
        }
    }
}