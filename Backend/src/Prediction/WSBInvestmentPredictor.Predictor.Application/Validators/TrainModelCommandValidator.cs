using FluentValidation;
using WSBInvestmentPredictor.Prediction.Application.Commands;

namespace WSBInvestmentPredictor.Prediction.Application.Validators;

public class TrainModelCommandValidator : AbstractValidator<TrainModelCommand>
{
    public TrainModelCommandValidator()
    {
        RuleFor(cmd => cmd.Data)
            .NotNull().WithMessage("Training data is required.")
            .Must(d => d.Count >= 50).WithMessage("At least 50 samples are required to train the model.");
    }
}
