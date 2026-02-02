using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Application.Services;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookTracking.UnitTests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;
    private readonly Mock<IAuditLogService> _mockAuditLogService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _mockAuthorRepository = new Mock<IAuthorRepository>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();

        _authorService = new AuthorService(
            _mockAuthorRepository.Object,
            _mockAuditLogService.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldCreateAuthor_WhenInputIsValid()
    {
        var authorDto = new AuthorDto { Name = "New Author" };
        var authorEntity = new Author { Id = Guid.NewGuid(), Name = "New Author" };

        _mockAuthorRepository.Setup(r => r.AddAsync(It.IsAny<Author>())).ReturnsAsync(authorEntity);
        _mockMapper.Setup(m => m.Map<AuthorDto>(authorEntity)).Returns(new AuthorDto { Id = authorEntity.Id, Name = authorEntity.Name });

        var result = await _authorService.CreateAuthorAsync(authorDto);

        result.Should().NotBeNull();
        result.Name.Should().Be("New Author");
        _mockAuthorRepository.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.Action == AuditType.Create)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldUpdate_WhenAuthorExistsAndNameChanged()
    {
        var authorDto = new AuthorDto { Id = Guid.NewGuid(), Name = "Updated Name" };
        var existingAuthor = new Author { Id = authorDto.Id, Name = "Old Name" };

        _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorDto.Id)).ReturnsAsync(existingAuthor);
        _mockAuthorRepository.Setup(r => r.UpdateAsync(existingAuthor)).ReturnsAsync(existingAuthor);
        _mockMapper.Setup(m => m.Map<AuthorDto>(existingAuthor)).Returns(authorDto);

        await _authorService.UpdateAuthorAsync(authorDto);

        existingAuthor.Name.Should().Be("Updated Name");
        _mockAuthorRepository.Verify(r => r.UpdateAsync(existingAuthor), Times.Once);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.Action == AuditType.Update)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldThrow_WhenAuthorNotFound()
    {
        var authorDto = new AuthorDto { Id = Guid.NewGuid(), Name = "Name" };
        _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorDto.Id)).ReturnsAsync((Author?)null);

        Func<Task> act = async () => await _authorService.UpdateAuthorAsync(authorDto);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Author not found");
    }

    [Fact]
    public async Task UpdateAuthorAsync_ShouldNotCommit_WhenNoChanges()
    {
        var authorDto = new AuthorDto { Id = Guid.NewGuid(), Name = "Name" };
        var existingAuthor = new Author { Id = authorDto.Id, Name = "Name" };

        _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorDto.Id)).ReturnsAsync(existingAuthor);
        _mockMapper.Setup(m => m.Map<AuthorDto>(existingAuthor)).Returns(authorDto);

        await _authorService.UpdateAuthorAsync(authorDto);

        _mockAuthorRepository.Verify(r => r.UpdateAsync(It.IsAny<Author>()), Times.Never);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.IsAny<AuditLogDto>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }


    [Fact]
    public async Task DeleteAuthorAsync_ShouldThrow_WhenAuthorNotFound()
    {
        var authorId = Guid.NewGuid();
        _mockAuthorRepository.Setup(r => r.GetByIdWithBooksAsync(authorId)).ReturnsAsync((Author?)null);

        Func<Task> act = async () => await _authorService.DeleteAuthorAsync(authorId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Author id {authorId} not found.");
    }

    [Fact]
    public async Task DeleteAuthorAsync_ShouldThrow_WhenAuthorHasBooks()
    {
        var authorId = Guid.NewGuid();
        var author = new Author { Id = authorId, Name = "Author", Books = new List<Book> { new Book { Isbn="1", Title="T", PublishDate=DateTime.UtcNow, Authors=new List<Author>(), IsActive=true } } };

        _mockAuthorRepository.Setup(r => r.GetByIdWithBooksAsync(authorId)).ReturnsAsync(author);

        Func<Task> act = async () => await _authorService.DeleteAuthorAsync(authorId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete author with associated books.");
    }
    
    [Fact]
    public async Task DeleteAuthorAsync_ShouldDelete_WhenAuthorHasNoBooks()
    {
        var authorId = Guid.NewGuid();
        var author = new Author { Id = authorId, Name = "Author", Books = new List<Book>() };

        _mockAuthorRepository.Setup(r => r.GetByIdWithBooksAsync(authorId)).ReturnsAsync(author);
        _mockAuthorRepository.Setup(r => r.DeleteAsync(author)).ReturnsAsync(author);

        await _authorService.DeleteAuthorAsync(authorId);

        _mockAuthorRepository.Verify(r => r.DeleteAsync(author), Times.Once);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.Action == AuditType.Delete)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
