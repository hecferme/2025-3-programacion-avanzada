using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class Book
{
    public int Id { get; set; }

    public string OriginalTitle { get; set; } = null!;

    public string? EnglishTitle { get; set; }

    public string? Edition { get; set; }

    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();
}
