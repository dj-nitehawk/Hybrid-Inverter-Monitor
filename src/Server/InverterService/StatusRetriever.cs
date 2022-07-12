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
            cmd.Result.Reset();
            if (queue.AddCommand(cmd))
                await Task.Delay(3000);
            else
                await Task.Delay(1000);
        }
    }
}