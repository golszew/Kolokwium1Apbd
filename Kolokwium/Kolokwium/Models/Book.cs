﻿using Kolokwium.Models.DTOs;

namespace Kolokwium.Models;

public class Book
{
    public int IdBook { get; set; }
    public string Title { get; set; }
    public List<AuthorDTO> Authors { get; set; }
}