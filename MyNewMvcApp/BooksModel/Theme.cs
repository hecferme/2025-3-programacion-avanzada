using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class Theme
{
    public int Id { get; set; }

    public string ThemeName { get; set; } = null!;

    public string? Subject { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
