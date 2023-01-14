namespace InverterMon.Server.BatteryService;

public sealed class AmpValQueue : Queue<float>
{
    public int FixedCapacity { get; }
    public AmpValQueue(int fixedCapacity)
    {
        FixedCapacity = fixedCapacity;
    }

    public new void Enqueue(float val)
    {
        if (val > 0)
        {
            base.Enqueue(val);
            if (Count > FixedCapacity)
            {
                Dequeue();
            }
        }
        else
        {
            Clear();
        }
    }

    public float GetAverage()
    {
        return Count > 0
               ? this.Average()
               : 0;
    }
}