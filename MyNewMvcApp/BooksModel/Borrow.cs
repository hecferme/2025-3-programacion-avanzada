using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class Borrow
{
    public int Id { get; set; }

    public DateOnly InitialDate { get; set; }

    public DateOnly FinalDate { get; set; }

    public DateOnly? RealDevolutionDate { get; set; }

    public int BookCopyId { get; set; }

    public int PersonId { get; set; }

    public virtual BookCopy BookCopy { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
