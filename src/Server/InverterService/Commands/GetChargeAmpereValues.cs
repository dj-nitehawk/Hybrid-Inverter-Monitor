namespace InverterMon.Server.InverterService.Commands;

class GetChargeAmpereValues : Command<List<string>>
{
    public override string CommandString { get; set; } = "QMCHGCR";
    public override bool IsTroublesomeCmd { get; } = true;

    public GetChargeAmpereValues(bool getUtilityValues)
    {
        Result.AddRange(new[] { "000" });

        if (getUtilityValues)
            CommandString = "QMUCHGCR";
    }

    public override void Parse(string responseFromInverter)
    {
        if (responseFromInverter.StartsWith("(NAK"))
            return;

        var parts = responseFromInverter[1..]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x[..3]);

        if (parts.Any())
            Result.Clear(); //remove default values

        Result.AddRange(parts);
    }
}