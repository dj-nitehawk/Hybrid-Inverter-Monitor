namespace InverterMon.Server.InverterService.Commands;

public interface ICommand
{
    string CommandString { get; set; }
    void Parse(string rawResponse);
    void Start();
    void End();
}

public abstract class Command<TResponseDto> : ICommand where TResponseDto : new()
{
    public abstract string CommandString { get; set; }
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

    public async Task WhileProcessing(CancellationToken c)
    {
        while (!c.IsCancellationRequested && !IsComplete && DateTime.Now.Subtract(startTime).TotalMilliseconds <= 3000)
            await Task.Delay(500, c);
    }
}