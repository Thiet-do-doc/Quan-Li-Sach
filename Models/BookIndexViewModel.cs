namespace BookManagement.Models;

public class BookIndexViewModel
{
    public IReadOnlyList<Book> Books { get; set; } = Array.Empty<Book>();
    public IReadOnlyList<string> Genres { get; set; } = Array.Empty<string>();
    public string? Search { get; set; }
    public string? Genre { get; set; }
    public int TotalBooks { get; set; }
    public int FilteredCount { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
