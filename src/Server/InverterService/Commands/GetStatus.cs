using InverterMon.Shared.Models;

namespace InverterMon.Server.InverterService.Commands;

public class GetStatus : Command<InverterStatus>
{
    public override string CommandString { get; set; } = "QPIGS";

    public override void Parse(string responseFromInverter)
    {
        //(232.0 50.1 232.0 50.1 0000 0000 000 476 27.02 000 100 0553 0000 000.0 27.00 00000 10011101 03 04 00000 101a\xc8\r

        if (IsCommandSuccessful(responseFromInverter))
        {
            string[]? parts = responseFromInverter[1..]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Result.GridVoltage = decimal.Parse(parts[0]);
            Result.OutputVoltage = decimal.Parse(parts[2]);
            Result.LoadWatts = int.Parse(parts[5]);
            Result.LoadPercentage = decimal.Parse(parts[6]);
            Result.BatteryVoltage = decimal.Parse(parts[8]);
            Result.BatteryChargeCurrent = int.Parse(parts[9]);
            Result.HeatSinkTemperature = int.Parse(parts[11]);
            Result.PVInputCurrent = decimal.Parse(parts[12]);
            Result.PVInputVoltage = decimal.Parse(parts[13]);
            Result.BatteryDischargeCurrent = int.Parse(parts[15]);
            Result.PVInputWatt = Convert.ToInt32(int.Parse(parts[19]) / 1.09);
        }
    }
}
