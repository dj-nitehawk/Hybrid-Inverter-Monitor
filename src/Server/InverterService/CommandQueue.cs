using InverterMon.Server.InverterService.Commands;
using System.Collections.Concurrent;
using ICommand = InverterMon.Server.InverterService.Commands.ICommand;

namespace InverterMon.Server.InverterService;

public class CommandQueue
{
    public bool IsAcceptingCommands { get; set; } = true;
    public GetStatus StatusCommand { get; } = new();

    readonly ConcurrentQueue<ICommand> toProcess = new();

    public bool AddCommands(params ICommand[] commands)
    {
        if (IsAcceptingCommands)
        {
            foreach (var cmd in commands)
                toProcess.Enqueue(cmd);
            return true;
        }
        return false;
    }

    public ICommand? GetCommand()
        => toProcess.TryPeek(out var command) ? command : null;

    public void RemoveCommand()
        => toProcess.TryDequeue(out _);
}
