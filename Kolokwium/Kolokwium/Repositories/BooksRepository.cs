using System.Data.SqlClient;
using Kolokwium.Models;
using Kolokwium.Models.DTOs;

namespace Kolokwium.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Book> GetBookAuthors(int id)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));
        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText =
            "Select b.pk as pk, b.title as title, first_name, last_name from books b join books_authors ba on b.pk = ba.fk_book join authors a on ba.fk_author = a.pk where b.pk = @pk";
        cmd.Parameters.AddWithValue("@pk", id);

        await con.OpenAsync();
        var reader = await cmd.ExecuteReaderAsync();

        var book = new Book();
        while (await reader.ReadAsync())
        {
            if (book.IdBook == 0)
            {
                book = new Book
                {
                    IdBook = reader.GetInt32(reader.GetOrdinal("pk")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Authors = new List<AuthorDTO>()
                };
                
                book.Authors.Add(new AuthorDTO
                {
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name"))
                });
                
            }
        }

        return book;
    }

    public async Task<bool> CheckIfBookExists(int id)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));

        using var cmd = new SqlCommand();

        cmd.Connection = con;
        cmd.CommandText = "SELECT 1 from books where pk = @pk";
        cmd.Parameters.AddWithValue("@pk", id);
        await con.OpenAsync();
        var reader = await cmd.ExecuteScalarAsync();
        return reader is not null;
    }

    public async Task<Book> AddNewBook(BookDTO bookDto)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));

        using var cmd = new SqlCommand();

        cmd.Connection = con;
        cmd.CommandText = "INSERT INTO Books (title) values @title; SELECT Scope identity as PK";
        cmd.Parameters.AddWithValue("@title", bookDto.Title);
        var result = await cmd.ExecuteScalarAsync();
        

        Book book = new Book
        {
            Title = bookDto.Title,
            Authors = new List<AuthorDTO>()
        };
        foreach (var author in bookDto.Authors)
        {
            await AddNewAuthor(author);
            book.Authors.Add(author);
        }

        return book;
    }

    public async Task<int> AddNewAuthor(AuthorDTO authorDto)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("Default"));

        using var cmd = new SqlCommand();
        var transaction = con.BeginTransaction();

        cmd.Connection = con;

        cmd.CommandText = "INSERT INTO authors (first_name, last_name) values(@first, @last); SELECT Scope identity as PK";
        cmd.Parameters.AddWithValue("@first", authorDto.FirstName);
        cmd.Parameters.AddWithValue("@last", authorDto.LastName);
        var result = await cmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
        {
            transaction.Rollback();
            throw new Exception("Error adding author");
        }

        return (int) result;
    }
}