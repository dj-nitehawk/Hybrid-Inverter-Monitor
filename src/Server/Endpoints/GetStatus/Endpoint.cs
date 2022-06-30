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

    public override async Task HandleAsync(CancellationToken c)
    {
        try
        {
            await SendAsync(GetDataStream(c), cancellation: c);
        }
        catch (TaskCanceledException)
        {
            //nothing to do here
        }
    }

    private async IAsyncEnumerable<InverterStatus> GetDataStream([EnumeratorCancellation] CancellationToken c)
    {
        Status? cmd = new();

        while (!c.IsCancellationRequested)
        {
            if (Env.IsDevelopment())
            {
                await Task.Delay(3000, c);
                cmd.Data.PVInputVoltage = Random.Shared.Next(300);
                cmd.Data.BatteryVoltage = Random.Shared.Next(26);
                cmd.Data.LoadWatts = Random.Shared.Next(3500);
                yield return cmd.Data;
            }
            else
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
}