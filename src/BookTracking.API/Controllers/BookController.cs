
using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public BookController(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Create([FromBody] CreateBookRequest request)
    {
        try
        {
            var bookDto = _mapper.Map<BookDto>(request);
            var createdBook = await _bookService.CreateBookAsync(bookDto);
            var bookResponse = _mapper.Map<BookResponse>(createdBook);
            return Ok(ApiResponse<BookResponse>.Success(bookResponse, 201, "Book created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BookResponse>.Failure(ex.Message, 400));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BookResponse>.Failure(ex.Message, 404));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BookResponse>.Failure($"An unexpected error occurred: {ex.Message} Inner: {ex.InnerException?.Message}", 500));
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult<BookResponse>> Update([FromBody] UpdateBookRequest request)
    {
        try
        {
            var bookDto = _mapper.Map<BookDto>(request);
            var updatedBook = await _bookService.UpdateBookAsync(bookDto);
            var bookResponse = _mapper.Map<BookResponse>(updatedBook);
            return Ok(ApiResponse<BookResponse>.Success(bookResponse, 200, "Book updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BookResponse>.Failure(ex.Message, 400));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BookResponse>.Failure(ex.Message, 404));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BookResponse>.Failure("An unexpected error occurred while updating the book.", 500));
        }

    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return Ok(ApiResponse<object>.Success(null, 200, "Book deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(ex.Message, 404));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure("An unexpected error occurred while deleting the book.", 500));
        }
    }
    [HttpGet("list")]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookResponse>>>> GetAll()
    {
        try
        {
            var books = await _bookService.GetAllBooksAsync();
            var bookResponses = _mapper.Map<IEnumerable<BookResponse>>(books);
            return Ok(ApiResponse<IEnumerable<BookResponse>>.Success(bookResponses, 200, "Books retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<BookResponse>>.Failure("An unexpected error occurred while retrieving books.", 500));
        }
    }
}
