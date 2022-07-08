namespace InverterMon.Server.InverterService.Commands;

public interface ICommand
{
    string CommandString { get; set; }
    bool IsComplete { get; set; }
    void Parse(string rawResponse);
}

internal abstract class Command<TResponseDto> : ICommand where TResponseDto : new()
{

    public DateTime StartTime { private get; set; }
    public bool TimedOut => DateTime.Now.Subtract(StartTime).TotalSeconds > 5;
    public TResponseDto Result { get; protected set; } = new();
    public bool IsComplete { get; set; }
    public abstract string CommandString { get; set; }

    public abstract void Parse(string responseFromInverter);

    protected bool IsCommandSuccessful(string responseFromInverter)
    {
        //Console.WriteLine("inverter response: " + responseFromInverter);
        return responseFromInverter[1..4] == "ACK";
    }

    public async Task WhileProcessing(CancellationToken c)
    {
        while (!IsComplete && !TimedOut)
            await Task.Delay(300, c);
    }
}