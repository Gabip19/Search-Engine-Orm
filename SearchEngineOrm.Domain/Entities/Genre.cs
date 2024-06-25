﻿using OrmLibrary.Attributes;

namespace SearchEngineOrm.Domain.Entities;

[Table("Genres")]
public class Genre
{
    public string GenreName { get; set; }
    public string Description { get; set; }
    [PrimaryKey]
    public Guid GenreId { get; set; }
}