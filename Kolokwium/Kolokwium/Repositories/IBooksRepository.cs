using Kolokwium.Models;
using Kolokwium.Models.DTOs;

namespace Kolokwium.Repositories;

public interface IBooksRepository
{
    Task<Book> GetBookAuthors(int id);
    Task<bool> CheckIfBookExists(int id);

    Task<Book> AddNewBook(BookDTO bookDto);

    Task<int> AddNewAuthor(AuthorDTO authorDto);

}