namespace InverterMon.Server.BatteryService;

public sealed class AmpValQueue
{
    readonly Queue<float> _queue = new();
    bool _lastChargingState;
    readonly int _fixedCapacity;

    public AmpValQueue(int fixedCapacity)
    {
        _fixedCapacity = fixedCapacity;
    }

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

        if (_queue.Count > _fixedCapacity)
            _queue.Dequeue();
    }

    public float GetAverage()
        => _queue.Count > 0
               ? _queue.Average()
               : 0;
}