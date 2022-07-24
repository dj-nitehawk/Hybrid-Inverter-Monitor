using InverterMon.Shared.Models;

namespace InverterMon.Server.InverterService.Commands;

internal class GetSettings : Command<CurrentSettings>
{
    public override string CommandString { get; set; } = "QPIRI";

    public override void Parse(string responseFromInverter)
    {
        // 1) 230.0 - grid rating voltage
        // 2) 15.2 - grid rating current
        // 3) 230.0 - ac output rating voltage
        // 4) 50.0 - ac output rating frequency
        // 5) 15.2 - ac output rating current
        // 6) 3500 - ac output rating apparant power
        // 7) 3500 - ac output rating active power
        // 8) 24.0 - batt rating voltage
        // 9) 23.5 - batt back to grid voltage
        // 10) 23.4 - batt discharge cut off voltage
        // 11) 28.8 - batt bulk charging voltage
        // 12) 27.0 - batt float charging voltage
        // 13) 2 - battery type (0:agm / 1:flooded / 2: user)
        // 14) 10 - max ac charging current
        // 15) 020 - max combined charging current
        // 16) 1 - input voltage range (0:appliance / 1:ups)
        // 17) 1 - output source priority (0:utility first / 1:solar first / 2:solar>battery>utility)
        // 18) 3 - charge priority (0:utility first /1:solar first / 2:solar & utility / 3:only solar)
        // 19) 1 - parallel max number
        // 20) 01 - machine type
        // 21) 0 - topology
        // 22) 0 - output mode
        // 23) 28.5 - back to battery use voltage
        // 24) 0 - pv ok for parallel
        // 25) 1 - pv power balance

        if (IsCommandSuccessful(responseFromInverter))
        {
            string[]? parts = responseFromInverter[1..]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Result.MaxACChargeCurrent = parts[14 - 1];
            Result.MaxCombinedChargeCurrent = parts[15 - 1];
            Result.OutputPriority = $"0{parts[17 - 1]}";
            Result.ChargePriority = $"0{parts[18 - 1]}";
        }
    }
}
