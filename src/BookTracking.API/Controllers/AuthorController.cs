using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly IMapper _mapper;

    public AuthorController(IAuthorService authorService, IMapper mapper)
    {
        _authorService = authorService;
        _mapper = mapper;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Create([FromBody] CreateAuthorRequest request)
    {
        try
        {
            var authorDto = _mapper.Map<AuthorDto>(request);
            var createdAuthor = await _authorService.CreateAuthorAsync(authorDto);
            var authorResponse = _mapper.Map<AuthorResponse>(createdAuthor);
            return Ok(ApiResponse<AuthorResponse>.Success(authorResponse, 201, "Author created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AuthorResponse>.Failure(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthorResponse>.Failure("An unexpected error occurred while creating the author.", 500));
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Update([FromBody] UpdateAuthorRequest request)
    {
        try
        {
            var authorDto = _mapper.Map<AuthorDto>(request);
            var updatedAuthor = await _authorService.UpdateAuthorAsync(authorDto);
            var authorResponse = _mapper.Map<AuthorResponse>(updatedAuthor);
            return Ok(ApiResponse<AuthorResponse>.Success(authorResponse, 200, "Author updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<AuthorResponse>.Failure(ex.Message, 404));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthorResponse>.Failure("An unexpected error occurred while updating the author.", 500));
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _authorService.DeleteAuthorAsync(id);
            return Ok(ApiResponse<object>.Success(null, 200, "Author deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Failure(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure("An unexpected error occurred while deleting the author.", 500));
        }
    }
}
