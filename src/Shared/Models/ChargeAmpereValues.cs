namespace InverterMon.Shared.Models;

public class ChargeAmpereValues
{
    public IEnumerable<string> CombinedCurrentValues { get; set; }
    public IEnumerable<string> UtilityCurrentValues { get; set; }
}