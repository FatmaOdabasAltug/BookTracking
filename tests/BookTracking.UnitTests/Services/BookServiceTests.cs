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

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;
    private readonly Mock<IAuditLogService> _mockAuditLogService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _mockBookRepository = new Mock<IBookRepository>();
        _mockAuthorRepository = new Mock<IAuthorRepository>();
        _mockAuditLogService = new Mock<IAuditLogService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();

        _bookService = new BookService(
            _mockBookRepository.Object,
            _mockAuthorRepository.Object,
            _mockAuditLogService.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task CreateBookAsync_ShouldCreateBook_WhenInputIsValid()
    {
        // Arrange
        var bookDto = new BookDto { Isbn = "123", Title = "Test Title", Authors = new List<Guid> { Guid.NewGuid() } };
        var authorEntity = new Author { Id = bookDto.Authors.First(), Name = "Author Name" };
        var bookEntity = new Book { Id = Guid.NewGuid(), Isbn = "123", Title = "Test Title", PublishDate = DateOnly.FromDateTime(DateTime.UtcNow), Authors = new List<Author> { authorEntity }, IsActive=true };

        _mockBookRepository.Setup(r => r.AnyByIsbnAsync(bookDto.Isbn)).ReturnsAsync(false);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author> { authorEntity });
        _mockMapper.Setup(m => m.Map<Book>(bookDto)).Returns(bookEntity);
        _mockBookRepository.Setup(r => r.AddAsync(bookEntity)).ReturnsAsync(bookEntity);
        _mockMapper.Setup(m => m.Map<BookDto>(bookEntity)).Returns(bookDto);

        // Act
        var result = await _bookService.CreateBookAsync(bookDto);

        // Assert
        result.Should().Be(bookDto);
        _mockBookRepository.Verify(r => r.AddAsync(bookEntity), Times.Once);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.IsAny<AuditLogDto>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBookAsync_ShouldThrow_WhenIsbnExists()
    {
        // Arrange
        var bookDto = new BookDto { Isbn = "123" };
        _mockBookRepository.Setup(r => r.AnyByIsbnAsync(bookDto.Isbn)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _bookService.CreateBookAsync(bookDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"The ISBN '{bookDto.Isbn}' is already registered.");
    }
    
    [Fact]
    public async Task CreateBookAsync_ShouldThrow_WhenAuthorsNotFound()
    {
        // Arrange
        var bookDto = new BookDto { Isbn = "123", Authors = new List<Guid> { Guid.NewGuid() } };
        _mockBookRepository.Setup(r => r.AnyByIsbnAsync(bookDto.Isbn)).ReturnsAsync(false);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author>()); // Returns empty list

        // Act
        Func<Task> act = async () => await _bookService.CreateBookAsync(bookDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("One or more authors not found.");
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldUpdateTitle_WhenTitleChanged()
    {
        // Arrange
        var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "New Title", Authors = new List<Guid>(), Isbn = "111", PublishDate = DateOnly.FromDateTime(DateTime.UtcNow) };
        var existingBook = new Book { Id = bookDto.Id, Title = "Old Title", Isbn = "111", Description = "Desc", PublishDate = DateOnly.MinValue, Authors = new List<Author>(), IsActive=true };
        
        _mockBookRepository.Setup(r => r.GetByIdWithAuthorsAsync(bookDto.Id)).ReturnsAsync(existingBook);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author>());
        _mockBookRepository.Setup(r => r.UpdateAsync(existingBook)).ReturnsAsync(existingBook);
        _mockMapper.Setup(m => m.Map<BookDto>(existingBook)).Returns(bookDto);

        // Act
        await _bookService.UpdateBookAsync(bookDto);

        // Assert
        existingBook.Title.Should().Be("New Title");
        _mockBookRepository.Verify(r => r.UpdateAsync(existingBook), Times.Once);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(log => log.PropertyName == "Title")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldThrow_WhenBookNotFound()
    {
        var bookDto = new BookDto { Id = Guid.NewGuid() };
        _mockBookRepository.Setup(r => r.GetByIdWithAuthorsAsync(bookDto.Id)).ReturnsAsync((Book?)null);

        Func<Task> act = async () => await _bookService.UpdateBookAsync(bookDto);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Book id {bookDto.Id} not found.");
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldUpdateDescription_WhenDescriptionChanged()
    {
        var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "Title", Description = "New Desc", Authors = new List<Guid>(), Isbn="1", PublishDate=DateOnly.FromDateTime(DateTime.UtcNow) };
        var existingBook = new Book { Id = bookDto.Id, Title = "Title", Description = "Old Desc", PublishDate = DateOnly.FromDateTime(DateTime.UtcNow), Authors = new List<Author>(), Isbn="1", IsActive=true };
        
        _mockBookRepository.Setup(r => r.GetByIdWithAuthorsAsync(bookDto.Id)).ReturnsAsync(existingBook);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author>());
        _mockBookRepository.Setup(r => r.UpdateAsync(existingBook)).ReturnsAsync(existingBook);
        _mockMapper.Setup(m => m.Map<BookDto>(existingBook)).Returns(bookDto);

        await _bookService.UpdateBookAsync(bookDto);

        existingBook.Description.Should().Be("New Desc");
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.PropertyName == "Description")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldUpdatePublishDate_WhenDateChanged()
    {
        var oldDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
        var newDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "Title", PublishDate = newDate, Authors = new List<Guid>(), Isbn="1", Description="D" };
        var existingBook = new Book { Id = bookDto.Id, Title = "Title", PublishDate = oldDate, Authors = new List<Author>(), Isbn="1", Description="D", IsActive=true };
        
        _mockBookRepository.Setup(r => r.GetByIdWithAuthorsAsync(bookDto.Id)).ReturnsAsync(existingBook);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author>());
        _mockBookRepository.Setup(r => r.UpdateAsync(existingBook)).ReturnsAsync(existingBook);
        _mockMapper.Setup(m => m.Map<BookDto>(existingBook)).Returns(bookDto);

        await _bookService.UpdateBookAsync(bookDto);

        existingBook.PublishDate.Should().Be(newDate);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.PropertyName == "PublishDate")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldUpdateAuthors_WhenAuthorsChanged()
    {
        var oldAuthor = new Author { Id = Guid.NewGuid(), Name = "Old Author" };
        var newAuthor = new Author { Id = Guid.NewGuid(), Name = "New Author" };
        
        var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "Title", Authors = new List<Guid> { newAuthor.Id }, Isbn="1", PublishDate=DateOnly.FromDateTime(DateTime.UtcNow), Description="D" };
        var existingBook = new Book { Id = bookDto.Id, Title = "Title", Authors = new List<Author> { oldAuthor }, Isbn="1", PublishDate=DateOnly.FromDateTime(DateTime.UtcNow), Description="D", IsActive=true };
        
        _mockBookRepository.Setup(r => r.GetByIdWithAuthorsAsync(bookDto.Id)).ReturnsAsync(existingBook);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author> { newAuthor });
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(It.Is<IEnumerable<Guid>>(ids => ids.Contains(oldAuthor.Id)))).ReturnsAsync(new List<Author> { oldAuthor }); // For audit log

        _mockBookRepository.Setup(r => r.UpdateAsync(existingBook)).ReturnsAsync(existingBook);
        _mockMapper.Setup(m => m.Map<BookDto>(existingBook)).Returns(bookDto);

        await _bookService.UpdateBookAsync(bookDto);

        existingBook.Authors.Should().Contain(newAuthor);
        existingBook.Authors.Should().NotContain(oldAuthor);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.PropertyName == "Authors")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ShouldNotCommit_WhenNoChanges()
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "Title", Authors = new List<Guid>(), Isbn="1", PublishDate=now, Description="D" };
        var existingBook = new Book { Id = bookDto.Id, Title = "Title", Authors = new List<Author>(), Isbn="1", PublishDate=now, Description="D", IsActive=true };
        
        _mockBookRepository.Setup(r => r.GetByIdWithAuthorsAsync(bookDto.Id)).ReturnsAsync(existingBook);
        _mockAuthorRepository.Setup(r => r.GetAuthorListAsync(bookDto.Authors)).ReturnsAsync(new List<Author>());
        _mockMapper.Setup(m => m.Map<BookDto>(existingBook)).Returns(bookDto);

        await _bookService.UpdateBookAsync(bookDto);

        _mockBookRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.IsAny<AuditLogDto>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteBookAsync_ShouldThrow_WhenBookNotFound()
    {
        var bookId = Guid.NewGuid();
        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book?)null);

        Func<Task> act = async () => await _bookService.DeleteBookAsync(bookId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Book id {bookId} not found.");
    }

    [Fact]
    public async Task DeleteBookAsync_ShouldDelete_WhenBookExists()
    {
        var bookId = Guid.NewGuid();
        var book = new Book { Id = bookId, Title = "Title", Isbn = "111", PublishDate = DateOnly.FromDateTime(DateTime.UtcNow), Authors = new List<Author>(), IsActive=true };

        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _mockBookRepository.Setup(r => r.DeleteAsync(book)).ReturnsAsync(book);

        await _bookService.DeleteBookAsync(bookId);

        _mockBookRepository.Verify(r => r.DeleteAsync(book), Times.Once);
        _mockAuditLogService.Verify(a => a.CreateLogAsync(It.Is<AuditLogDto>(l => l.Action == AuditType.Delete)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once); 
    }
}
