using InverterMon.Server.InverterService;

namespace InverterMon.Server.Endpoints.Settings.GetChargeAmpereValues;

public class Endpoint : EndpointWithoutRequest<Shared.Models.ChargeAmpereValues>
{
    public CommandQueue Queue { get; set; }

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
                CombinedCurrentValues = new[] { "010", "020", "040" },
                UtilityCurrentValues = new[] { "04", "10", "20" }
            });
            return;
        }

        var cmd1 = new InverterService.Commands.GetChargeAmpereValues(true);
        var cmd2 = new InverterService.Commands.GetChargeAmpereValues(false);
        Queue.Commands.Enqueue(cmd1);
        Queue.Commands.Enqueue(cmd2);
        await Task.WhenAll(
            cmd1.WhileProcessing(c),
            cmd2.WhileProcessing(c));

        if (cmd1.IsComplete && cmd2.IsComplete)
        {
            await SendAsync(new()
            {
                CombinedCurrentValues = cmd2.Result,
                UtilityCurrentValues = cmd1.Result
            });
        }
        else
        {
            ThrowError("Unable to read charge current values in a timely manner!");
        }
    }
}