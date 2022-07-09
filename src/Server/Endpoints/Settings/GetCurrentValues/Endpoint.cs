using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.Settings.GetCurrentValues;

public class Endpoint : EndpointWithoutRequest<SettingsStatus>
{
    public CommandQueue Queue { get; set; }

    public override void Configure()
    {
        Get("settings/get-current-values");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var cmd = new CurrentSettings();
        Queue.Commands.Enqueue(cmd);
        await cmd.WhileProcessing(c);
        if (cmd.IsComplete)
            await SendAsync(cmd.Result);
        else
            ThrowError("Unable to read current settings in a timely manner!");
    }
}