﻿using Library.Interfaces;
using Library.Models;
 
namespace Library.Services;

public class BookcaseService
{
    private readonly Bookcase _bookcase = new();
    private readonly double _percentage = 0.01;

    private IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();
    private IFees Fees { get; set; }

    public BookcaseService()
    {
        if (DateTime.Today.DayOfWeek == DayOfWeek.Tuesday)
        {
            Fees = new DicountedFeesService();
        }
        else
        {
            Fees = new FeesService();
        }
    }

    public void AddBook(Book book, int bookCopies)
    {
        for (int i = 0; i < bookCopies; i++) 
        {
            var bookCopy = new BookCopy();
            book.BookCopies.Add(bookCopy);
        }
        _bookcase.Books.Add(book);
        Console.WriteLine("Book added successfully and the book copies with ids: ");

        foreach( var bookCopy in book.BookCopies)
        { 
            Console.WriteLine($" {bookCopy.Id} "); 
        }
    }

    public List<Book> GetAllBooks()
    { 
        var availableBooks = _bookcase.Books
            .ToList();

        if (availableBooks.Count > 0)
        {
            foreach (var book in availableBooks)
            {
                Console.WriteLine($"Title: {book.Title}, Rental Price: {book.RentalPrice}, ISBN: {book.ISBN}");
            }
        }
        else
        {
            Console.WriteLine("Sorry, we don't have available books in the library.");
        }

        return availableBooks;
    }

    public List<BookCopy> GetAllBookCopies(string isbn)
    {
        var book = _bookcase.Books
            .FirstOrDefault(x => x.ISBN == isbn);

        if (book == null)
        {
            Console.WriteLine("Sorry, we don't have this book in the library.");
            return new List<BookCopy>(); 
        }

        var bookCopies = book.BookCopies
            .Where(x => !x.IsBorrowed)
            .ToList();

        Console.WriteLine($"We have {bookCopies.Count()} available book copies in the library: ");
        foreach (var bookCopy in bookCopies) 
        {
            Console.WriteLine($" {bookCopy.Id} ");
        }
        return bookCopies;
    }

    public void BorrowBook(string isbn)
    {
        var book = _bookcase.Books
            .FirstOrDefault(x => x.ISBN == isbn);

        if (book == null) 
        {
            Console.WriteLine("The book you want to rent is not available.");
            return;
        }

        var availableBookCopy = book.BookCopies.FirstOrDefault(x => !x.IsBorrowed);
        if (availableBookCopy == null)
        {
            Console.WriteLine("We don't have available copies for the book.");
            return;
        }
        
        availableBookCopy.IsBorrowed = true;
        availableBookCopy.BorrowDate = DateTime.Now;
        Console.WriteLine($"You borrow the book with id: {availableBookCopy.Id}!");
    }

    public double ReturnBook(Guid id)
    {
        var borrowedBook = _bookcase.Books
            .FirstOrDefault(x => x.BookCopies
            .Any(x => x.Id == id));

        var borrowedBookCopy = borrowedBook.BookCopies
            .FirstOrDefault(x => x.Id == id);

        if (borrowedBookCopy == null)
        {
            Console.WriteLine("Book copy not found.");
            return 0.0;
        }

        var currentDate = DateTime.Today;
        double totalAmount = 0.0;

        totalAmount = Fees.CalculateTotalAmount(borrowedBookCopy.BorrowDate, borrowedBook.RentalPrice);
      
        borrowedBookCopy.IsBorrowed = false;
        return totalAmount;
    }
}
