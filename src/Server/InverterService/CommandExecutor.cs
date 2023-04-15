using System.Diagnostics;
using ICommand = InverterMon.Server.InverterService.Commands.ICommand;

namespace InverterMon.Server.InverterService;

internal class CommandExecutor : BackgroundService
{
    private readonly CommandQueue queue;
    private readonly ILogger<CommandExecutor> log;
    private readonly IConfiguration confing;

    public CommandExecutor(CommandQueue queue, IConfiguration config, ILogger<CommandExecutor> log)
    {
        this.queue = queue;
        confing = config;
        this.log = log;

        log.LogInformation("connecting to the inverter...");

        var sw = new Stopwatch();
        sw.Start();

        while (!Connect() && sw.Elapsed.TotalMinutes <= 5)
            Thread.Sleep(10000);

        if (sw.Elapsed.TotalMinutes >= 5)
        {
            log.LogInformation("inverter connecting timed out!");
        }
    }

    private bool Connect()
    {
        var devPath = confing["LaunchSettings:DeviceAddress"] ?? "/dev/hidraw0";

        if (!Inverter.Connect(devPath, log))
        {
            return false;
        }
        else
        {
            log.LogInformation("connected to inverter at: [{adr}]", devPath);
            return true;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var delay = 0;
        var timeout = TimeSpan.FromMinutes(5);

        while (!ct.IsCancellationRequested && delay <= timeout.TotalMilliseconds)
        {
            var cmd = queue.GetCommand();
            if (cmd is not null)
            {
                try
                {
                    await ExecuteCommand(cmd, ct);
                    queue.IsAcceptingCommands = true;
                    delay = 0;
                    queue.RemoveCommand();
                }
                catch (Exception x)
                {
                    queue.IsAcceptingCommands = false;
                    log.LogError("command [{cmd}] failed with reason [{msg}]", cmd.CommandString, x.Message);
                    await Task.Delay(delay += 1000);
                }
            }
            else
            {
                await Task.Delay(500, ct);
            }
        }
        log.LogError("command execution halted due to excessive failures!");
    }

    private static async Task ExecuteCommand(ICommand command, CancellationToken ct)
    {
        command.Start();
        await Inverter.Write(command.CommandString, ct);
        command.Parse(await Inverter.Read(ct));
        command.End();
    }
}