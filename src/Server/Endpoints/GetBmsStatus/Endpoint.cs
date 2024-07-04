using System.Runtime.CompilerServices;
using InverterMon.Server.BatteryService;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.GetBmsStatus;

public class Endpoint : EndpointWithoutRequest<object>
{
    public JkBms Bms { get; set; }

    public override void Configure()
    {
        Get("bms-status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        try
        {
            if (Bms.IsConnected)
                await SendAsync(GetDataStream(c), cancellation: c);
            else
                await SendNotFoundAsync(c);
        }
        catch (TaskCanceledException)
        {
            //nothing to do here
        }
    }

    async IAsyncEnumerable<BMSStatus> GetDataStream([EnumeratorCancellation] CancellationToken c)
    {
        while (!c.IsCancellationRequested)
        {
            yield return Bms.Status;

            await Task.Delay(1000, c);
        }
    }
}