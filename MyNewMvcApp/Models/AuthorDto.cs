namespace MyNewMvcApp.Models
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public List<string?> Books { get; set; } = new();
    }
}
