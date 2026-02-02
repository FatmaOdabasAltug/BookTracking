using System.Data;
using BookTracking.API.Models;
using FluentValidation;

namespace BookTracking.API.Validators;

public class UpdateAuthorRequestValidator : AbstractValidator<UpdateAuthorRequest>
{
    public UpdateAuthorRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("AuthorId is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters long.");
    }
}
