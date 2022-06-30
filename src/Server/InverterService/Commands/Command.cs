namespace InverterMon.Server.InverterService.Commands;

public interface ICommand
{
    string CommandString { get; }
    bool IsComplete { get; set; }
    void Parse(string rawResponse);
}

internal abstract class Command<TResponseDto> : ICommand where TResponseDto : new()
{

    public DateTime StartTime { private get; set; }
    public bool TimedOut => DateTime.Now.Subtract(StartTime).TotalSeconds > 3;
    public TResponseDto Data { get; } = new();
    public bool IsComplete { get; set; }
    public abstract string CommandString { get; }

    public abstract void Parse(string rawResponse);
}