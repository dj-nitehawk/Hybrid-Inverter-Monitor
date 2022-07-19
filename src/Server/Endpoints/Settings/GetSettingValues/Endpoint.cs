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
        var cmd1 = new GetSettings();
        cmd1.Result.SystemSpec = UserSettings.ToSystemSpec();

        if (Env.IsDevelopment())
        {
            cmd1.Result.ChargePriority = "03";
            cmd1.Result.MaxACChargeCurrent = "10";
            cmd1.Result.MaxCombinedChargeCurrent = "020";
            cmd1.Result.OutputPriority = "02";
            cmd1.Result.CombinedAmpereValues = new[] { "010", "020", "030" };
            cmd1.Result.UtilityAmpereValues = new[] { "04", "10", "20" };
            await SendAsync(cmd1.Result);
            return;
        }

        var cmd2 = new GetChargeAmpereValues(true);
        var cmd3 = new GetChargeAmpereValues(false);

        Queue.AddCommands(cmd1, cmd2, cmd3);

        await Task.WhenAll(
            cmd1.WhileProcessing(c),
            cmd2.WhileProcessing(c),
            cmd3.WhileProcessing(c));

        if (cmd1.IsComplete && cmd2.IsComplete && cmd3.IsComplete)
        {
            cmd1.Result.UtilityAmpereValues = cmd2.Result;
            cmd1.Result.CombinedAmpereValues = cmd3.Result;
            await SendAsync(cmd1.Result);
        }
        else
        {
            ThrowError("Unable to read settings in a timely manner!");
        }
    }
}