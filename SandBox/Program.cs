if (!port.CanWrite)
{
    Console.WriteLine("couldn't open port!!!");
    return;
}

var status = new Status();

var count = 0;
while (count < 30)
{
    var statusResult = SendCommand("QPIGS");
    status.Parse(statusResult);

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

port.Close();

