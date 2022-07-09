using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.Settings.SetChargePriority;

public class Endpoint : Endpoint<ChargePriorityRequest, bool>
{
    public CommandQueue Queue { get; set; }

    public override void Configure()
    {
        Get("settings/charge-priority/{Priority}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ChargePriorityRequest r, CancellationToken c)
    {
        var cmd = new ChargePriority(r.Priority);

        Queue.Commands.Enqueue(cmd);

        await cmd.WhileProcessing(c);

        await SendAsync(cmd.Result);
    }
}