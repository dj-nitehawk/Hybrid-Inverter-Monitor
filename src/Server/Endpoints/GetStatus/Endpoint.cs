using InverterMon.Server.InverterService;
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

    async IAsyncEnumerable<InverterStatus> GetDataStream([EnumeratorCancellation] CancellationToken c)
    {
        var blank = new InverterStatus();

        while (!c.IsCancellationRequested)
        {
            if (Env.IsDevelopment())
            {
                var status = Queue.StatusCommand.Result;
                status.OutputVoltage = Random.Shared.Next(240);
                status.LoadWatts = Random.Shared.Next(3500);
                status.LoadPercentage = Random.Shared.Next(100);
                status.BatteryVoltage = Random.Shared.Next(24);
                status.BatteryChargeCurrent = Random.Shared.Next(20);
                status.BatteryDischargeCurrent = Random.Shared.Next(300);
                status.HeatSinkTemperature = Random.Shared.Next(300);
                status.PVInputCurrent = Random.Shared.Next(300);
                status.PVInputVoltage = Random.Shared.Next(300);
                status.PVInputWatt = Random.Shared.Next(1000);
                status.PV_MaxCapacity = 1000;
                status.BatteryCapacity = 100;

                yield return status;
            }
            else
            {
                yield return Queue.IsAcceptingCommands
                                 ? Queue.StatusCommand.Result
                                 : blank;
            }
            await Task.Delay(1000, c);
        }
    }
}