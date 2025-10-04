using System;
using System.Collections.Generic;

namespace ProgramacionAvanzada.Books.Model;

public partial class xyz
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public DateTime? created_at { get; set; }
}
