using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Shared.Models;
using System.Runtime.CompilerServices;

namespace InverterMon.Server.Endpoints.GetStatus;

public class Endpoint : EndpointWithoutRequest<object>
{
    private static InverterStatus blank = new InverterStatus();

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
                cmd.Data.OutputVoltage = Random.Shared.Next(250);
                cmd.Data.LoadWatts = Random.Shared.Next(3500);
                cmd.Data.BatteryVoltage = Random.Shared.Next(28);
                cmd.Data.BatteryChargeCurrent = Random.Shared.Next(20);
                cmd.Data.BatteryDischargeCurrent = 10; Random.Shared.Next(10);
                cmd.Data.HeatSinkTemperature = Random.Shared.Next(300);
                cmd.Data.PVInputCurrent = Random.Shared.Next(300);
                cmd.Data.PVInputVoltage = Random.Shared.Next(300);
                cmd.Data.PVInputWatt = Random.Shared.Next(1800);
                yield return cmd.Data;
                await Task.Delay(1000, c);
            }
            else
            {
                cmd.IsComplete = false;
                cmd.StartTime = DateTime.Now;
                Queue.Commands.Enqueue(cmd);

                while (!cmd.IsComplete && !cmd.TimedOut)
                    await Task.Delay(300, c);

                if (cmd.IsComplete)
                    yield return cmd.Data;
                else
                    yield return blank;

                await Task.Delay(2000, c);
            }
        }
    }
}