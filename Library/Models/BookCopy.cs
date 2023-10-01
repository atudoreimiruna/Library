namespace Library.Models;

public class BookCopy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsBorrowed { get; set; } = false;
    public DateTime BorrowDate { get; set; }
}
