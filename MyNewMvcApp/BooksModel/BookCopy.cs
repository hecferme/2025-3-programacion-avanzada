using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class BookCopy
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string? ISBN { get; set; }

    public DateOnly? DateCreated { get; set; }

    public string? Serial { get; set; }

    public string? Language { get; set; }

    public string? Provider { get; set; }

    public bool? IsLost { get; set; }

    public DateTime? LostDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
