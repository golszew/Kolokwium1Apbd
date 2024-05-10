using Kolokwium.Models.DTOs;
using Kolokwium.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[ApiController]
[Route("/api/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetAuthors(int id)
    {
        if (!await _booksRepository.CheckIfBookExists(id))
        {
            return NotFound($"Book with id {id} was not found");
        }

        try
        {
            var result =  await _booksRepository.GetBookAuthors(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(BookDTO bookDto)
    {
        var result = await _booksRepository.AddNewBook(bookDto);
        result
    }
}