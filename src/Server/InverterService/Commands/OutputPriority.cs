namespace InverterMon.Server.InverterService.Commands;

internal class OutputPriority : Command<bool>
{
    public override string CommandString { get; set; } = "POP";

    public OutputPriority(string priorityValue)
    {
        CommandString += priorityValue;
    }

    public override void Parse(string responseFromInverter)
    {
        Result = IsCommandSuccessful(responseFromInverter);
    }
}
