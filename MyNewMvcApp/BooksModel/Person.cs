using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Sex { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}
