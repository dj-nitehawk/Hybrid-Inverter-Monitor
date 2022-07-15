using InverterMon.Server.Persistance;

namespace InverterMon.Server.InverterService;

internal class StatusRetriever : BackgroundService
{
    private readonly CommandQueue queue;
    private readonly Database db;

    public StatusRetriever(CommandQueue queue, Database db)
    {
        this.queue = queue;
        this.db = db;
    }

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        var cmd = queue.StatusCommand;

        while (!c.IsCancellationRequested)
        {
            if (queue.IsAcceptingCommands)
            {
                queue.AddCommand(cmd);
                _ = db.UpdateTodaysPVGeneration(cmd, c);
            }

            await Task.Delay(3000);
        }
    }
}