using InverterMon.Server.InverterService;

namespace InverterMon.Server.Endpoints.Settings.SetSettingValue;

public class Endpoint : Endpoint<Shared.Models.SetSetting, bool>
{
    public CommandQueue Queue { get; set; }

    public override void Configure()
    {
        Get("settings/set-setting/{Command}/{Value}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Shared.Models.SetSetting r, CancellationToken c)
    {
        var cmd = new InverterService.Commands.SetSetting(r.Command, r.Value);
        Queue.AddCommands(cmd);
        await cmd.WhileProcessing(c);
        await SendAsync(cmd.Result);
    }
}