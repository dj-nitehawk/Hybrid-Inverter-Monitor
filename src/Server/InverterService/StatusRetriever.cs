namespace InverterMon.Server.InverterService;

internal class StatusRetriever : BackgroundService
{
    private readonly CommandQueue queue;

    public StatusRetriever(CommandQueue queue)
        => this.queue = queue;

    protected override async Task ExecuteAsync(CancellationToken c)
    {
        var cmd = queue.StatusCommand;

        while (!c.IsCancellationRequested)
        {
            queue.AddCommand(cmd);
            await Task.Delay(3000);
        }
    }
}