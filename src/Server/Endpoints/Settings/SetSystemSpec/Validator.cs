using FluentValidation;
using InverterMon.Shared.Models;

namespace InverterMon.Server.Endpoints.Settings.SetSystemSpec;

public class Validator : Validator<SystemSpec>
{
    public Validator()
    {
        RuleFor(x => x.PV_MaxCapacity)
            .GreaterThan(100);

        RuleFor(x => x.SunlightStartHour)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(24)
            .Must((s, h) => h < s.SunlightEndHour).WithMessage("Sunlight start hour must be earlier than end hour!");

        RuleFor(x => x.SunlightEndHour)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(24)
            .Must((s, h) => h > s.SunlightStartHour).WithMessage("Sunlight end hour must be later than start hour!"); ;

        //todo: display validation errors on ui
    }
}