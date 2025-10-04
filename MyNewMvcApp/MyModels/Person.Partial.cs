using System;

namespace ProgramacionAvanzada.Books.Model
{
    public partial class Person
    {
        public string FullName => $"{Name} {LastName}";
        public string FullLastNameName => $"{LastName}, {Name}";
    }
}
