using System.Text.Json.Serialization;

namespace InverterMon.Shared.Models;

public class BMSStatus
{
    [JsonPropertyName("a")]
    public float AvailableCapacity => PackCapacity / 100f * CapacityPct;

    [JsonPropertyName("b")]
    public float AvgCellVoltage => Cells.Values.Average();

    [JsonPropertyName("c")]
    public float AvgCurrentAmps { get; set; }

    [JsonPropertyName("d")]
    public ushort CapacityPct { get; set; }

    [JsonPropertyName("e")]
    public float CellDiff => MaxCell.Value - MinCell.Value;

    [JsonPropertyName("f")]
    public Dictionary<byte, float> Cells { get; set; } = new(); //key: cell number //val: cell voltage

    [JsonPropertyName("g")]
    public double CRate => Math.Round(AvgCurrentAmps / PackCapacity, 2, MidpointRounding.AwayFromZero);

    [JsonPropertyName("h")]
    public bool IsCharging { get; set; }

    [JsonPropertyName("i")]
    public bool IsDisCharging => !IsCharging;

    [JsonPropertyName("j")]
    public KeyValuePair<byte, float> MaxCell => Cells.MaxBy(x => x.Value);

    [JsonPropertyName("k")]
    public KeyValuePair<byte, float> MinCell => Cells.MinBy(x => x.Value);

    [JsonPropertyName("l")]
    public float MosTemp { get; set; }

    [JsonPropertyName("m")]
    public uint PackCapacity { get; set; }

    [JsonPropertyName("n")]
    public float PackVoltage { get; set; }

    [JsonPropertyName("o")]
    public float Temp1 { get; set; }

    [JsonPropertyName("p")]
    public float Temp2 { get; set; }

    [JsonPropertyName("q")]
    public ushort TimeHrs { get; set; }

    [JsonPropertyName("r")]
    public int TimeMins { get; set; }

    [JsonPropertyName("s")]
    public bool IsWarning { get; set; }

    [JsonPropertyName("t")]
    public double AvgPowerWatts => Math.Round(AvgCurrentAmps * PackVoltage, 0, MidpointRounding.AwayFromZero);
}