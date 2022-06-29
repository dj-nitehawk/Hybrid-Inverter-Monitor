using InverterMon.Inverter;

var status = Inverter.Status;
var count = 0;

while (count < 30)
{
    status.Update();

    Console.Write(@$"
Load Watts: {status.LoadWatt}
Load Percentage: {status.LoadPercentage}%
PV Voltage: {status.PVInputVoltage}V
PV Current: {status.PVInputCurrent}A
PV Watts: {status.PVInputWatt}
Battery Voltage: {status.BatteryVoltage}V");
    Console.SetCursorPosition(0, 1);

    count++;
    await Task.Delay(1000);
}