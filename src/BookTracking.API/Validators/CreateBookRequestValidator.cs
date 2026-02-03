using BookTracking.API.Models;
using FluentValidation;

namespace BookTracking.API.Validators;
public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Isbn).NotEmpty().WithMessage("ISBN is required.")
            .Matches(@"^\d{13}$").WithMessage("ISBN must consist of 13 digits.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(200).WithMessage("Title must be at most 200 characters long.");
        RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description must be at most 1000 characters long.");
        RuleFor(x => x.PublishDate).NotEmpty().WithMessage("Publish date is required.").LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Publish date cannot be in the future.");
        RuleFor(x => x.Authors)
            .NotEmpty().WithMessage("At least one author must be selected.")
            .Must(x => x.Count > 0).WithMessage("The author list cannot be empty.");
    }
}