namespace InverterMon.Shared.Models;

public class BMSStatus
{
    public Dictionary<byte, float> Cells { get; set; } = new(); //key: cell number //val: cell voltage
    public float AvgCellVoltage => Cells.Values.Average();
    public KeyValuePair<byte, float> MinCell => Cells.MinBy(x => x.Value);
    public KeyValuePair<byte, float> MaxCell => Cells.MaxBy(x => x.Value);
    public float CellDiff => MaxCell.Value - MinCell.Value;
    public ushort MosTemp { get; set; }
    public ushort Temp1 { get; set; }
    public ushort Temp2 { get; set; }
    public float PackVoltage { get; set; }
    public bool IsCharging { get; set; }
    public bool IsDisCharging => !IsCharging;
    public float AvgCurrentAmps { get; set; }
    public ushort CapacityPct { get; set; }
    public uint PackCapacity { get; set; }
    public float AvailableCapacity => PackCapacity / 100f * CapacityPct;
    public ushort TimeHrs { get; set; }
    public int TimeMins { get; set; }
    public double CRate => Math.Round(AvgCurrentAmps / PackCapacity, 2, MidpointRounding.AwayFromZero);
}