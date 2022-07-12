using InverterMon.Server.InverterService.Commands;
using System.Collections.Concurrent;

namespace InverterMon.Server.InverterService;

public class CommandQueue
{
    public GetStatus StatusCommand { get; } = new();

    private readonly ConcurrentQueue<ICommand> toProcess = new();

    public void AddCommand(ICommand command)
    {
        toProcess.Enqueue(command);
    }

    public ICommand? GetCommand()
    {
        return toProcess.TryDequeue(out var command) ? command : null;
    }
}
