using FluentValidation;
using CMCS.Models;

public class Validator : AbstractValidator<Claims>
{
    public Validator()
    {
        //ensure HoursWorked does not exceed 45
        RuleFor(c => c.HoursWorked)
            .LessThanOrEqualTo(45) //set maximum allowed hours
            .WithMessage("Maximum working hours reached"); //error message

        //rule to ensure OvertimeHours does not exceed 10, only if it has a value
        RuleFor(c => c.OvertimeHours)
            .LessThanOrEqualTo(10) //set maximum allowed overtime hours
            .When(c => c.OvertimeHours.HasValue) //rule is applied only when OvertimeHours is not null
            .WithMessage("Maximum overtime hours reached."); //error message if rule is violated
    }
}
