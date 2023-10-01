namespace Library.Models;

public class Book 
{
    public string Title { get; set; }
    public string ISBN { get; set; }
    public double RentalPrice { get; set; }
    public List<BookCopy> BookCopies = new List<BookCopy>();
}
