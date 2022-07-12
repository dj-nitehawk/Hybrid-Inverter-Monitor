using InverterMon.Server.InverterService.Commands;
using System.Collections.Concurrent;

namespace InverterMon.Server.InverterService;

public class CommandQueue
{
    public GetStatus StatusCommand { get; } = new();
    public bool IsAcceptingCommands { get; set; } = true;

    private readonly ConcurrentQueue<ICommand> toProcess = new();

    public bool AddCommand(ICommand command)
    {
        if (IsAcceptingCommands)
        {
            toProcess.Enqueue(command);
            return true;
        }
        return false;
    }

    public ICommand? GetCommand()
    {
        return toProcess.TryDequeue(out var command) ? command : null;
    }
}
