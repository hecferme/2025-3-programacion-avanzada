using System;

namespace ProgramacionAvanzada.Books.Model
{
    public partial class BookTheme
    {
        public int BookId { get; set; }
        public int ThemeId { get; set; }

        public virtual Book Book { get; set; } = null!;
        public virtual Theme Theme { get; set; } = null!;
    }
}
