namespace InverterMon.Server.InverterService.Commands;

class SetSetting : Command<bool>
{
    public override string CommandString { get; set; }
    public override bool IsTroublesomeCmd { get; } = true;

    public SetSetting(string settingName, string settingValue)
    {
        CommandString = settingName + settingValue;
    }

    public override void Parse(string responseFromInverter)
    {
        Result = responseFromInverter[1..4] == "ACK";
    }
}