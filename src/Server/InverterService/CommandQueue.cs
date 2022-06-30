using InverterMon.Server.InverterService.Commands;
using System.Collections.Concurrent;

namespace InverterMon.Server.InverterService;

public class CommandQueue
{
    public ConcurrentQueue<ICommand> Commands { get; } = new();
}
