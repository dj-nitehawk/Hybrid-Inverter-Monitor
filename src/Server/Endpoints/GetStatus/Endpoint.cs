using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Shared.Models;
using System.Runtime.CompilerServices;

namespace InverterMon.Server.Endpoints.GetStatus;

public class Endpoint : EndpointWithoutRequest<object>
{
    public CommandQueue Queue { get; set; }

    public override void Configure()
    {
        Get("status");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken c)
        => SendAsync(GetDataStream(c), cancellation: c);

    private async IAsyncEnumerable<InverterStatus> GetDataStream([EnumeratorCancellation] CancellationToken c)
    {
        Status? cmd = new();

        while (!c.IsCancellationRequested)
        {
            cmd.IsComplete = false;
            cmd.StartTime = DateTime.Now;
            Queue.Commands.Enqueue(cmd);

            while (!cmd.IsComplete && !cmd.TimedOut)
                await Task.Delay(500, c);

            if (cmd.IsComplete)
                yield return cmd.Data;

            await Task.Delay(5000, c);
        }
    }
}