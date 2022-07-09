using InverterMon.Server.InverterService;
using InverterMon.Shared.Models;
using System.Runtime.CompilerServices;

namespace InverterMon.Server.Endpoints.GetStatus;

public class Endpoint : EndpointWithoutRequest<object>
{
    private static readonly InverterStatus blank = new();

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
        InverterService.Commands.GetStatus? cmd = new();

        while (!c.IsCancellationRequested)
        {
            if (Env.IsDevelopment())
            {
                cmd.Result.OutputVoltage = Random.Shared.Next(250);
                cmd.Result.LoadWatts = Random.Shared.Next(3500);
                cmd.Result.LoadPercentage = Random.Shared.Next(100);
                cmd.Result.BatteryVoltage = Random.Shared.Next(28);
                cmd.Result.BatteryChargeCurrent = Random.Shared.Next(20);
                cmd.Result.BatteryDischargeCurrent = 10; _ = Random.Shared.Next(10);
                cmd.Result.HeatSinkTemperature = Random.Shared.Next(300);
                cmd.Result.PVInputCurrent = Random.Shared.Next(300);
                cmd.Result.PVInputVoltage = Random.Shared.Next(300);
                cmd.Result.PVInputWatt = Random.Shared.Next(1800);
                yield return cmd.Result;
                await Task.Delay(1000, c);
            }
            else
            {
                cmd.IsComplete = false;
                cmd.StartTime = DateTime.Now;
                Queue.Commands.Enqueue(cmd);

                await cmd.WhileProcessing(c);

                yield return cmd.IsComplete ? cmd.Result : blank;

                await Task.Delay(2000, c);
            }
        }
    }
}