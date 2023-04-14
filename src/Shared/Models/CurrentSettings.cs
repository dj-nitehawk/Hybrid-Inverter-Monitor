namespace InverterMon.Shared.Models;

public class CurrentSettings
{
    public string ChargePriority { get; set; } = "000";
    public string OutputPriority { get; set; } = "000";
    public string MaxACChargeCurrent { get; set; } = "000";
    public string MaxCombinedChargeCurrent { get; set; } = "000";

    private decimal _backToGrid;
    public decimal BackToGridVoltage { get => _backToGrid; set => _backToGrid = RoundToHalfPoints(value); }

    private decimal _dischargeCuttOff;
    public decimal DischargeCuttOffVoltage { get => _dischargeCuttOff; set => _dischargeCuttOff = RoundToOneDecimalPoint(value); }

    private decimal _bulkVoltage;
    public decimal BulkChargeVoltage
    {
        get => _bulkVoltage;
        set => _bulkVoltage = RoundToOneDecimalPoint(value < _floatVoltage ? _floatVoltage : value);
    }

    private decimal _floatVoltage;
    public decimal FloatChargeVoltage
    {
        get => _floatVoltage;
        set => _floatVoltage = RoundToOneDecimalPoint(value > _bulkVoltage ? _bulkVoltage : value);
    }

    private decimal _backToBattery;
    public decimal BackToBatteryVoltage { get => _backToBattery; set => _backToBattery = RoundToHalfPoints(value); }

    public SystemSpec SystemSpec { get; set; } = new();

    private static decimal RoundToHalfPoints(decimal value)
        => Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2;

    private static decimal RoundToOneDecimalPoint(decimal value)
        => Math.Round(value, 1, MidpointRounding.AwayFromZero);
}
