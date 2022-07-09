using InverterMon.Shared.Models;

namespace InverterMon.Server.InverterService.Commands;

internal class GetStatus : Command<InverterStatus>
{
    public override string CommandString { get; set; } = "QPIGS";

    public override void Parse(string responseFromInverter)
    {
        //(232.0 50.1 232.0 50.1 0000 0000 000 476 27.02 000 100 0553 0000 000.0 27.00 00000 10011101 03 04 00000 101a\xc8\r

        string[]? parts = responseFromInverter[1..]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Result.GridVoltage = decimal.Parse(parts[0]);
        //Data.GridFrequency = decimal.Parse(parts[1]);
        Result.OutputVoltage = decimal.Parse(parts[2]);
        //Data.OutputFrequency = decimal.Parse(parts[3]);
        //Data.LoadVA = int.Parse(parts[4]);
        Result.LoadWatts = int.Parse(parts[5]);
        Result.LoadPercentage = decimal.Parse(parts[6]);
        //Data.BusVoltage = decimal.Parse(parts[7]);
        Result.BatteryVoltage = decimal.Parse(parts[8]);
        Result.BatteryChargeCurrent = int.Parse(parts[9]);
        //Data.BatteryCapacity = int.Parse(parts[10]);
        Result.HeatSinkTemperature = int.Parse(parts[11]);
        Result.PVInputCurrent = decimal.Parse(parts[12]);
        Result.PVInputVoltage = decimal.Parse(parts[13]);
        //Data.SCCVoltage = decimal.Parse(parts[14]);
        Result.BatteryDischargeCurrent = int.Parse(parts[15]);
        //Data.PVOrACFeed = parts[16][0];
        //Data.LoadOn = parts[16][3];
        //Data.SCCOn = parts[16][1];
        //Data.ACChargeOn = parts[16][2];
        Result.PVInputWatt = Convert.ToInt32(int.Parse(parts[19]) / 1.09);
    }
}
