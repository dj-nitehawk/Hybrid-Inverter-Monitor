namespace InverterMon.Server.InverterService.Commands;

internal class ChargePriority : Command<bool>
{
    public override string CommandString { get; set; } = "PCP";

    public ChargePriority(string priorityValue)
    {
        CommandString += priorityValue;
    }

    public override void Parse(string responseFromInverter)
    {
        Result = IsCommandSuccessful(responseFromInverter);
    }
}
