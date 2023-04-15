using InverterMon.Server.InverterService;
using InverterMon.Server.Persistance.Settings;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.Settings.GetChargeAmpereValues;

public class Endpoint : EndpointWithoutRequest<ChargeAmpereValues>
{
    public CommandQueue Queue { get; set; }
    public UserSettings UserSettings { get; set; }

    public override void Configure()
    {
        Get("settings/get-charge-ampere-values");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        if (Env.IsDevelopment())
        {
            await SendAsync(new()
            {
                CombinedAmpereValues = new[] { "010", "020", "030" },
                UtilityAmpereValues = new[] { "04", "10", "20" }
            });
            return;
        }

        var cmd1 = new InverterService.Commands.GetChargeAmpereValues(false);
        var cmd2 = new InverterService.Commands.GetChargeAmpereValues(true);
        Queue.AddCommands(cmd1, cmd2);

        await Task.WhenAll(
            cmd1.WhileProcessing(c, 5000),
            cmd2.WhileProcessing(c, 5000));

        await SendAsync(new()
        {
            CombinedAmpereValues = cmd1.Result,
            UtilityAmpereValues = cmd2.Result
        });
    }
}