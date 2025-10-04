using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
