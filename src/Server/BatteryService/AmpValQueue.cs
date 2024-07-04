namespace InverterMon.Server.BatteryService;

public sealed class AmpValQueue(int fixedCapacity)
{
    readonly Queue<float> _queue = new();
    bool _lastChargingState;

    public void Store(float ampReading, bool chargingState)
    {
        if (ampReading == 0 || _lastChargingState != chargingState)
        {
            _queue.Clear();
            _lastChargingState = chargingState;

            return;
        }

        _lastChargingState = chargingState;
        _queue.Enqueue(ampReading);

        if (_queue.Count > fixedCapacity)
            _queue.Dequeue();
    }

    public float GetAverage()
        => _queue.Count > 0
               ? _queue.Average()
               : 0;
}