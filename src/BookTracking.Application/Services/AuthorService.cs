using AutoMapper;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Entities;
using BookTracking.Domain.Enums;
using BookTracking.Domain.Interfaces;

namespace BookTracking.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthorService(IAuthorRepository authorRepository, IAuditLogService auditLogService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AuthorDto> CreateAuthorAsync(AuthorDto dto)
    {
        var createdAuthor = await _authorRepository.AddAsync(new Author
        {
            Name = dto.Name
        });
        await _auditLogService.CreateLogAsync(new AuditLogDto
        {
            Action = AuditType.Create,
            EntityType = EntityType.Author,
            EntityId = createdAuthor.Id,
            Description = $"New author created: {createdAuthor.Name}"
        });
        await _unitOfWork.CommitAsync();
        return _mapper.Map<AuthorDto>(createdAuthor);
    }

    public async Task<AuthorDto> UpdateAuthorAsync(AuthorDto dto)
    {
        var currentAuthorEntity = await _authorRepository.GetByIdAsync(dto.Id);
        if (currentAuthorEntity == null)
            throw new ArgumentException("Author not found");

        if(currentAuthorEntity.Name != dto.Name)
        {
            currentAuthorEntity.Name = dto.Name;
            var updatedAuthor = await _authorRepository.UpdateAsync(currentAuthorEntity);
            await _auditLogService.CreateLogAsync(new AuditLogDto
            {
                Action = AuditType.Update,
                EntityType = EntityType.Author,
                EntityId = updatedAuthor.Id,
                Description = $"Author name changed from '{currentAuthorEntity.Name}' to '{updatedAuthor.Name}'",
                PropertyName = "Name",
                OldValue = currentAuthorEntity.Name,
                NewValue = updatedAuthor.Name
            });
            await _unitOfWork.CommitAsync();
            return _mapper.Map<AuthorDto>(updatedAuthor);
        }
        return _mapper.Map<AuthorDto>(currentAuthorEntity);
    }

    public async Task DeleteAuthorAsync(Guid id)
    {
        var authorEntity = await _authorRepository.GetByIdWithBooksAsync(id);
        if (authorEntity == null)
            throw new InvalidOperationException($"Author id {id} not found.");

        if(authorEntity.Books != null && authorEntity.Books.Any())
            throw new InvalidOperationException("Cannot delete author with associated books.");

        var deletedAuthorEntity = await _authorRepository.DeleteAsync(authorEntity);
        await _auditLogService.CreateLogAsync(new AuditLogDto
        {
            Action = AuditType.Delete,
            EntityType = EntityType.Author,
            EntityId = deletedAuthorEntity.Id,
            Description = $"Author deleted: {deletedAuthorEntity.Name}"
        });
        await _unitOfWork.CommitAsync();
    }


}