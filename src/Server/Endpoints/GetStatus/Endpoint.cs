using InverterMon.Server.InverterService;
using InverterMon.Shared.Models;
using System.Runtime.CompilerServices;

namespace InverterMon.Server.Endpoints.GetStatus;

public class Endpoint : EndpointWithoutRequest<object>
{
    public override void Configure()
    {
        Get("inverter-status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        await SendAsync(GetDataStream(c));
    }

    private static async IAsyncEnumerable<InverterStatus> GetDataStream([EnumeratorCancellation] CancellationToken cancellation)
    {
        while (!cancellation.IsCancellationRequested)
        {
            try
            {
                _ = await Inverter.Status.Update();
            }
            catch
            {
                Console.WriteLine("Read error occurred!");
            }
            yield return Inverter.Status.Data;
        }
    }
}