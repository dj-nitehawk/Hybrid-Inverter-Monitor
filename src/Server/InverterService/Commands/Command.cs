// ReSharper disable UnassignedGetOnlyAutoProperty

namespace InverterMon.Server.InverterService.Commands;

public interface ICommand
{
    string CommandString { get; set; }
    bool IsTroublesomeCmd { get; }
    void Parse(string rawResponse);
    void Start();
    void End();
}

public abstract class Command<TResponseDto> : ICommand where TResponseDto : new()
{
    public abstract string CommandString { get; set; }
    public virtual bool IsTroublesomeCmd { get; }
    public TResponseDto Result { get; protected set; } = new();
    public bool IsComplete { get; private set; }

    public abstract void Parse(string responseFromInverter);

    protected DateTime startTime = DateTime.Now;

    public void Start()
    {
        startTime = DateTime.Now;
        IsComplete = false;
    }

    public void End()
        => IsComplete = true;

    public async Task WhileProcessing(CancellationToken c, int timeoutMillis = Constants.StatusPollingFrequencyMillis)
    {
        while (!c.IsCancellationRequested && !IsComplete && DateTime.Now.Subtract(startTime).TotalMilliseconds <= timeoutMillis)
            await Task.Delay(500, c);
    }
}