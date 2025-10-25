using System;

namespace ProgramacionAvanzada.Books.Model
{
    public partial class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public virtual Book? Book { get; set; }
        public virtual Author? Author { get; set; }
    }
}
