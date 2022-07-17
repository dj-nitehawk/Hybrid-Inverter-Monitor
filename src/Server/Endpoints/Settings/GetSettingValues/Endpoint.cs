using InverterMon.Server.InverterService;
using InverterMon.Server.InverterService.Commands;
using InverterMon.Server.Persistance.Settings;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.Settings.GetSettingValues;

public class Endpoint : EndpointWithoutRequest<CurrentSettings>
{
    public CommandQueue Queue { get; set; }
    public UserSettings UserSettings { get; set; }

    public override void Configure()
    {
        Get("settings/get-setting-values");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var cmd = new GetSettings();
        cmd.Result.SystemSpec = UserSettings.ToSystemSpec();

        if (Env.IsDevelopment())
        {
            cmd.Result.ChargePriority = "03";
            cmd.Result.MaxACChargeCurrent = "10";
            cmd.Result.MaxCombinedChargeCurrent = "020";
            cmd.Result.OutputPriority = "02";
            await SendAsync(cmd.Result);
            return;
        }

        Queue.AddCommand(cmd);

        await cmd.WhileProcessing(c);

        if (cmd.IsComplete)
            await SendAsync(cmd.Result);
        else
            ThrowError("Unable to read current settings in a timely manner!");
    }
}