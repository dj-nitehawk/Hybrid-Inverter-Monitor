namespace InverterMon.Server.InverterService.Commands;

internal class SetSetting : Command<bool>
{
    public override string CommandString { get; set; }

    public SetSetting(string settingName, string settingValue)
    {
        CommandString = settingName + settingValue;
    }

    public override void Parse(string responseFromInverter)
    {
        Result = responseFromInverter[1..4] == "ACK";
    }
}