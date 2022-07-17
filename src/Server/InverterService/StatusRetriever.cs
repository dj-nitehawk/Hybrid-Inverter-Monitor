using InverterMon.Server.Persistance;
using InverterMon.Server.Persistance.Settings;

namespace InverterMon.Server.InverterService;

internal class StatusRetriever : BackgroundService
{
    private readonly CommandQueue queue;
    private readonly Database db;
    private readonly UserSettings userSettings;

    public StatusRetriever(CommandQueue queue, Database db, UserSettings userSettings)
    {
        this.queue = queue;
        this.db = db;
        this.userSettings = userSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        var cmd = queue.StatusCommand;

        while (!c.IsCancellationRequested)
        {
            if (queue.IsAcceptingCommands)
            {
                cmd.Result.PVMaxCapacity = userSettings.PVMaxCapacity; //feels hacky. find a better solution.
                queue.AddCommand(cmd);
                _ = db.UpdateTodaysPVGeneration(cmd, c);
            }
            await Task.Delay(3000);
        }
    }
}