namespace InverterMon.Server.BatteryService;

public sealed class AmpValQueue
{
    private readonly int capacity;
    private readonly Queue<float> queue = new();
    private bool lastChargingState;

    public AmpValQueue(int fixedCapacity)
    {
        capacity = fixedCapacity;
    }

    public void Store(float ampReading, bool chargingState)
    {
        if (ampReading == 0 || lastChargingState != chargingState)
        {
            queue.Clear();
            lastChargingState = chargingState;
            return;
        }

        lastChargingState = chargingState;
        queue.Enqueue(ampReading);

        if (queue.Count > capacity)
            queue.Dequeue();
    }

    public float GetAverage()
    {
        return queue.Count > 0
               ? queue.Average()
               : 0;
    }
}