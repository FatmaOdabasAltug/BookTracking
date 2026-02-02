using BookTracking.API.Models;
using BookTracking.API.Validators;
using FluentAssertions;
using Xunit;

namespace BookTracking.UnitTests.Validators;

public class CreateBookRequestValidatorTests
{
    private readonly CreateBookRequestValidator _validator;

    public CreateBookRequestValidatorTests()
    {
        _validator = new CreateBookRequestValidator();
    }

    [Fact]
    public void Validate_ShouldPass_ForValidRequest()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Isbn = "1234567890123",
            Title = "Valid Book",
            Description = "A valid description",
            PublishDate = DateTime.Today.AddYears(-1),
            Authors = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenIsbnIsTooShort()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Isbn = "22", // The problematic case reported by user
            Title = "Invalid Book",
            PublishDate = DateTime.Today,
            Authors = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Isbn" && e.ErrorMessage.Contains("ISBN must consist of 13 digits"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenIsbnHasNonDigits()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Isbn = "123456789012a", 
            Title = "Invalid Book",
            PublishDate = DateTime.Today,
            Authors = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Isbn" && e.ErrorMessage.Contains("digits"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenIsbnEmpty()
    {
         var request = new CreateBookRequest
        {
            Isbn = "", 
            Title = "Invalid Book",
            PublishDate = DateTime.Today,
            Authors = new List<Guid> { Guid.NewGuid() }
        };

        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Isbn" && e.ErrorMessage.Contains("required"));
    }
}
