
using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
    {
        try
        {
            var bookDto = _mapper.Map<BookDto>(request);
            await _bookService.CreateBookAsync(bookDto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred while creating the book.", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] UpdateBookRequest request)
    {
        try
        {
            var bookDto = _mapper.Map<BookDto>(request);
            await _bookService.UpdateBookAsync(bookDto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred while updating the book.", details = ex.Message });
        }

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred while deleting the book.", details = ex.Message });
        }
    }
}
