using System.Diagnostics;
using ICommand = InverterMon.Server.InverterService.Commands.ICommand;

namespace InverterMon.Server.InverterService;

class CommandExecutor : BackgroundService
{
    readonly CommandQueue queue;
    readonly ILogger<CommandExecutor> logger;
    readonly string _devPath = "/dev/hidraw0";
    readonly bool _isTroubleMode;
    readonly string _mppPath = "/usr/local/bin/mpp-solar";

    public CommandExecutor(CommandQueue queue, IConfiguration config, ILogger<CommandExecutor> log)
    {
        this.queue = queue;
        logger = log;
        _devPath = config["LaunchSettings:DeviceAddress"] ?? _devPath;
        _isTroubleMode = config["LaunchSettings:TroubleMode"] == "yes";
        _mppPath = config["LaunchSettings:MppSolarPath"] ?? _mppPath;

        log.LogInformation("connecting to the inverter...");

        var sw = new Stopwatch();
        sw.Start();

        while (!Connect() && sw.Elapsed.TotalMinutes <= 5)
            Thread.Sleep(10000);

        if (sw.Elapsed.TotalMinutes >= 5)
            log.LogInformation("inverter connecting timed out!");
    }

    bool Connect()
    {
        if (!Inverter.Connect(_devPath, logger))
            return false;

        logger.LogInformation("connected to inverter at: [{adr}]", _devPath);

        return true;
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
                    logger.LogError("command [{cmd}] failed with reason [{msg}]", cmd.CommandString, x.Message);
                    await Task.Delay(delay += 1000);
                }
            }
            else
                await Task.Delay(500, ct);
        }
        logger.LogError("command execution halted due to excessive failures!");
    }

    async Task ExecuteCommand(ICommand command, CancellationToken ct)
    {
        if (_isTroubleMode && command.IsTroublesomeCmd)
        {
            Inverter.Disconnect();
            using var process = new Process();
            process.StartInfo.FileName = _mppPath;
            process.StartInfo.Arguments = $"-p {_devPath} -o raw -c {command.CommandString}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            command.Start();
            var output = await process.StandardOutput.ReadToEndAsync(ct);
            var result = output.ParseCli()[1..^1];
            command.Parse(result);
            command.End();
            await process.WaitForExitAsync(ct);
            Inverter.Connect(_devPath, logger);
        }
        else
        {
            command.Start();
            await Inverter.Write(command.CommandString, ct);
            command.Parse(await Inverter.Read(ct));
            command.End();
        }
    }
}