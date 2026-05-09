using ApiRefactor.Contracts.Requests;
using FluentValidation;

namespace ApiRefactor.Validation;

public sealed class UpsertWaveRequestValidator : AbstractValidator<UpsertWaveRequest>
{
    public UpsertWaveRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.WaveDate)
            .LessThanOrEqualTo(_ => DateTime.UtcNow.AddYears(1))
            .WithMessage("WaveDate cannot be more than one year in the future.");
    }
}
