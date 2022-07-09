namespace InverterMon.Server.InverterService.Commands;

internal class GetChargeAmpereValues : Command<List<string>>
{
    public override string CommandString { get; set; } = "QMCHGCR";

    public GetChargeAmpereValues(bool getUtilityValues)
    {
        if (getUtilityValues)
            CommandString = "QMUCHGCR";
    }

    public override void Parse(string responseFromInverter)
    {
        var parts = responseFromInverter[1..]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x[0..3]);

        Result.AddRange(parts);
    }
}