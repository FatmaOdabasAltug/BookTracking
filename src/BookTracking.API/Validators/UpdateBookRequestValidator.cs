using System.Data;
using BookTracking.API.Models;
using FluentValidation;

namespace BookTracking.API.Validators;
public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Book ID is required.");
        RuleFor(x => x.Isbn).NotEmpty().WithMessage("ISBN is required.").Length(13).WithMessage("ISBN must be 13 characters long.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(200).WithMessage("Title must be at most 200 characters long.");
        RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description must be at most 1000 characters long.");
        RuleFor(x => x.PublishDate).NotEmpty().WithMessage("Publish date is required.").LessThanOrEqualTo(DateTime.Today).WithMessage("Publish date cannot be in the future.");
        RuleFor(x => x.Authors)
            .NotEmpty().WithMessage("At least one author must be selected.")
            .Must(x => x.Count > 0).WithMessage("The author list cannot be empty.");
    }
}