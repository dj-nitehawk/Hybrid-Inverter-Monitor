namespace InverterMon.Server.InverterService;

internal class StatusRetriever : BackgroundService
{
    private readonly CommandQueue queue;
    private readonly Database.Database db;

    public StatusRetriever(CommandQueue queue, Database.Database db)
    {
        this.queue = queue;
        this.db = db;
    }

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        var cmd = queue.StatusCommand;

        while (!c.IsCancellationRequested)
        {
            queue.AddCommand(cmd);
            _ = db.UpdateTodaysPVGeneration(cmd, c);
            await Task.Delay(3000);
        }
    }
}