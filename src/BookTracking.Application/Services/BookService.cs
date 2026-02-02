using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Domain.Interfaces;

namespace BookTracking.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IAuditLogService auditLogService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BookDto> CreateBookAsync(BookDto bookDto)
    {
        if (await _bookRepository.AnyByIsbnAsync(bookDto.Isbn))
            throw new InvalidOperationException($"The ISBN '{bookDto.Isbn}' is already registered.");

        var authorEntityList = await _authorRepository.GetAuthorListAsync(bookDto.Authors);
        if (authorEntityList.Count() != bookDto.Authors.Count)
            throw new KeyNotFoundException("One or more authors not found.");

        var bookEntity = _mapper.Map<Book>(bookDto);
        bookEntity.Authors = authorEntityList.ToList();
        var createdBook = await _bookRepository.AddAsync(bookEntity);
        await _auditLogService.CreateLogAsync(new AuditLogDto
        {
            Action = AuditType.Create,
            EntityType = EntityType.Book,
            EntityId = createdBook.Id,
            Description = $"New book created: {createdBook.Title} (ISBN: {createdBook.Isbn})"
        });
        await _unitOfWork.CommitAsync();
        return _mapper.Map<BookDto>(createdBook);
    }

    public async Task<BookDto> UpdateBookAsync(BookDto bookDto)
    {
        var updateFlag = false;
        var existingBookEntity = await _bookRepository.GetByIdWithAuthorsAsync(bookDto.Id);
        if (existingBookEntity == null)
            throw new KeyNotFoundException($"Book id {bookDto.Id} not found.");


        if (existingBookEntity.Title != bookDto.Title)
        {
            updateFlag = true;

            await _auditLogService.CreateLogAsync(new AuditLogDto
            {
                Action = AuditType.Update,
                EntityType = EntityType.Book,
                EntityId = existingBookEntity.Id,
                Description = $"Book title changed from '{existingBookEntity.Title}' to '{bookDto.Title}'",
                PropertyName = "Title",
                OldValue = existingBookEntity.Title,
                NewValue = bookDto.Title
            });
            
            existingBookEntity.Title = bookDto.Title;
        }

        if (existingBookEntity.Description != bookDto.Description)
        {
            updateFlag = true;
            await _auditLogService.CreateLogAsync(new AuditLogDto
            {
                Action = AuditType.Update,
                EntityType = EntityType.Book,
                EntityId = existingBookEntity.Id,
                Description = $"Book ('{existingBookEntity.Title}') description changed from '{existingBookEntity.Description}' to '{bookDto.Description}'",
                PropertyName = "Description",
                OldValue = existingBookEntity.Description,
                NewValue = bookDto.Description
            });
            existingBookEntity.Description = bookDto.Description;
        } 

        if (existingBookEntity.PublishDate != bookDto.PublishDate)
        {
            updateFlag = true;
            await _auditLogService.CreateLogAsync(new AuditLogDto
            {
                Action = AuditType.Update,
                EntityType = EntityType.Book,
                EntityId = existingBookEntity.Id,
                Description = $"Book ('{existingBookEntity.Title}') publish date changed from '{existingBookEntity.PublishDate}' to '{bookDto.PublishDate}'",
                PropertyName = "PublishDate",
                OldValue = existingBookEntity.PublishDate.ToString("o"),
                NewValue = bookDto.PublishDate.ToString("o")
            });
            existingBookEntity.PublishDate = bookDto.PublishDate;
        }

        var newAuthorIds = bookDto.Authors;
        var newAuthorEntityList = await _authorRepository.GetAuthorListAsync(newAuthorIds);
        if (newAuthorEntityList.Count() != newAuthorIds.Count)
            throw new KeyNotFoundException("One or more authors not found.");
        var currentAuthorIds = existingBookEntity.Authors.Select(a => a.Id).ToList();

        if(!currentAuthorIds.SequenceEqual(newAuthorIds))// Authors have changed
        {
            updateFlag = true;
            var addedAuthors = newAuthorIds.Except(currentAuthorIds).ToList();
            var currentAuthorEntityList = await _authorRepository.GetAuthorListAsync(currentAuthorIds);
            await _auditLogService.CreateLogAsync(new AuditLogDto
                {
                    Action = AuditType.Update,
                    EntityType = EntityType.Book,
                    EntityId = existingBookEntity.Id,
                    Description = $"Book ('{existingBookEntity.Title}') authors changed from {string.Join(", ", currentAuthorEntityList.Where(a => currentAuthorIds.Contains(a.Id)).Select(a => a.Name))} to  {string.Join(", ", newAuthorEntityList.Where(a => newAuthorIds.Contains(a.Id)).Select(a => a.Name))}",
                    PropertyName = "Authors",
                    OldValue = string.Join(", ", currentAuthorIds.Select(id => currentAuthorEntityList.First(a => a.Id == id).Name)),
                    NewValue = string.Join(", ", newAuthorIds.Select(id => newAuthorEntityList.First(a => a.Id == id).Name))
                });

            existingBookEntity.Authors.Clear();
            foreach (var authorId in newAuthorIds)
            {
                existingBookEntity.Authors.Add(newAuthorEntityList.First(a => a.Id == authorId));
            }
        }
        if (!updateFlag)
            return _mapper.Map<BookDto>(existingBookEntity); // No changes detected
        var updatedBook = await _bookRepository.UpdateAsync(existingBookEntity);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<BookDto>(updatedBook);
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var deletedBookEntity = await _bookRepository.GetByIdAsync(id);
        if (deletedBookEntity == null)
            throw new KeyNotFoundException($"Book id {id} not found.");
        var deletedBook = await _bookRepository.DeleteAsync(deletedBookEntity);
        await _auditLogService.CreateLogAsync(new AuditLogDto
        {
            Action = AuditType.Delete,
            Description = $"Book ('{deletedBook.Title}') deleted.",
            EntityType = EntityType.Book,
            EntityId = deletedBook.Id,
        });
        await _unitOfWork.CommitAsync();
    }

}
