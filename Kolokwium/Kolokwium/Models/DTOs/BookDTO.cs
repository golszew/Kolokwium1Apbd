﻿namespace Kolokwium.Models.DTOs;

public class BookDTO
{
    public string Title { get; set; }
    public List<AuthorDTO> Authors { get; set; }
}