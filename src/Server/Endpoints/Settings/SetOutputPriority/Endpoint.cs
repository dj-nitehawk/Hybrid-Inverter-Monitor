using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.Settings.SetOutputPriority;

public class Endpoint : Endpoint<OutputPriorityRequest, bool>
{
    public CommandQueue Queue { get; set; }

    public override void Configure()
    {
        Get("settings/output-priority/{Priority}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(OutputPriorityRequest r, CancellationToken c)
    {
        var cmd = new OutputPriority(r.Priority);

        Queue.Commands.Enqueue(cmd);

        await cmd.WhileProcessing(c);

        await SendAsync(cmd.Result);
    }
}