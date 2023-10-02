using Library.Interfaces;
using Library.Models;
using Library.Services;

namespace Library.Tests;

[TestFixture]
public class BookcaseServiceTests
{
    private BookcaseService _bookcaseService;
    private readonly double _percentage = 0.01;
    private StringWriter consoleOutput;
    private TextWriter originalConsoleOut;

    [SetUp]
    public void SetUp()
    {
        _bookcaseService = new BookcaseService();

        consoleOutput = new StringWriter();
        originalConsoleOut = Console.Out;
        Console.SetOut(consoleOutput);
    }

    [TearDown]
    public void RestoreConsoleOutput()
    {
        Console.SetOut(originalConsoleOut);
        consoleOutput.Dispose();
    }

    [Test]
    public void AddBook_Should_AddTheBookInTheBookcase()
    {
        // Arrange
        var book = new Book { Title = "Pride and Prejudice", ISBN = "9781853260001", RentalPrice = 26.0 };
        var bookCopies = 5;

        // Act
        _bookcaseService.AddBook(book, bookCopies);

        // Assert
        // Check if the book was added to the bookcase
        var books = _bookcaseService.GetAllBooks();
        Assert.IsTrue(books.Contains(book));

        // Check if the correct number of book copies were added to the book
        Assert.AreEqual(bookCopies, book.BookCopies.Count);

        // Check if all book copies have unique IDs
        var distinctIds = book.BookCopies.Select(bc => bc.Id).Distinct();
        Assert.AreEqual(bookCopies, distinctIds.Count());

        // Check if all book copies have not been borrowed
        foreach (var bookCopy in book.BookCopies)
        {
            Assert.IsFalse(bookCopy.IsBorrowed);
        }
    }

    [Test]
    public void GetAllBooks_Should_ReturnListOfBooks()
    {
        // Arrange 
        var book1 = new Book { Title = "Pride and Prejudice", ISBN = "9781853260001", RentalPrice = 26.0 };
        var book2 = new Book { Title = "The Great Gatsby", ISBN = "9781847496140", RentalPrice = 35.0 };

        _bookcaseService.AddBook(book1, 3); 
        _bookcaseService.AddBook(book2, 2); 

        // Act
        var books = _bookcaseService.GetAllBooks();

        // Assert
        Assert.AreEqual(2, books.Count); 
        Assert.IsTrue(books.Any(b => b.Title == "Pride and Prejudice" && b.ISBN == "9781853260001"));
        Assert.IsTrue(books.Any(b => b.Title == "The Great Gatsby" && b.ISBN == "9781847496140"));
    }

    [Test]
    public void GetAllBooks_ShouldHandleNoAvailableBooks()
    {
        // Arrange
        var bookService = new BookcaseService();

        // Act
        var availableBooks = bookService.GetAllBooks();

        // Assert
        Assert.IsEmpty(availableBooks);
    }

    [Test]
    public void GetAllBookCopies_Should_ReturnAvailableBookCopiesForTheBook()
    {
        // Arrange 
        var book = new Book { Title = "Pride and Prejudice", ISBN = "9781853260001", RentalPrice = 26.0 };
        var availableCopies = 5;
        var borrowedCopies = 3;

        // Add available book copies
        _bookcaseService.AddBook(book, availableCopies + borrowedCopies);

        // Add borrowed book copies
        for (int i = 0; i < borrowedCopies; i++)
        {
            _bookcaseService.BorrowBook("9781853260001");
        }

        // Act
        var availableBookCopies = _bookcaseService.GetAllBookCopies("9781853260001");

        // Assert
        // Verify that the method returns the expected available book copies
        Assert.AreEqual(availableCopies, availableBookCopies.Count);
        Assert.IsTrue(availableBookCopies.All(copy => !copy.IsBorrowed));
    }

    [Test]
    public void GetAllBookCopies_ShouldHandleBookNotInLibrary()
    {
        // Arrange
        var bookService = new BookcaseService();

        // Act
        var availableCopies = bookService.GetAllBookCopies("9781853260001");

        // Assert
        Assert.IsEmpty(availableCopies);
        string capturedOutput = consoleOutput.ToString().Trim();
        Assert.AreEqual("Sorry, we don't have this book in the library.", capturedOutput);
    }

    [Test]
    public void BorrowBook_Should_BorrowBookCopyOfTheBook()
    {
        // Arrange 
        var book = new Book { Title = "Pride and Prejudice", ISBN = "9781853260001", RentalPrice = 26.0 };
        var availableCopies = 5;

        // Add available book copies
        _bookcaseService.AddBook(book, availableCopies);

        // Act
        _bookcaseService.BorrowBook("9781853260001");

        // Assert
        // Verify that a book copy was successfully borrowed
        var availableBookCopies = _bookcaseService.GetAllBookCopies("9781853260001");

        // Assert
        // Verify that the method returns the expected available book copies
        Assert.AreEqual(availableCopies-1, availableBookCopies.Count);
    }

    [Test]
    public void BorrowBook_ShouldHandleBookNotAvailable()
    {
        // Arrange
        var bookService = new BookcaseService();

        // Act
        bookService.BorrowBook("9781853260001");

        // Assert
        string capturedOutput = consoleOutput.ToString().Trim();
        Assert.AreEqual("The book you want to rent is not available.", capturedOutput);
    }

    [Test]
    public void ReturnBook_Should_ReturnBookCopy([Values(20, 25, 30)] int numberOfDays)
    {
        // Arrange 
        var book = new Book { Title = "Pride and Prejudice", ISBN = "9781853260001", RentalPrice = 26.0 };
        var copies = 1;

        // Add available book copies
        _bookcaseService.AddBook(book, copies);

        // Borrow the book
        _bookcaseService.BorrowBook("9781853260001");

        // Mock DateTime using Moq
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.SetupGet(dt => dt.Now).Returns(DateTime.Now.AddDays(numberOfDays));

        // Replace the DateTime provider in the service with the mocked one
        _bookcaseService.DateTimeProvider = mockDateTime.Object;

        // Act
        var totalAmount = _bookcaseService.ReturnBook(book.BookCopies[0].Id); // Return the book with the specific copy ID 

        // Assert
        var returnedCopy = book.BookCopies[0];
        Assert.IsFalse(returnedCopy.IsBorrowed); // Check if the book copy is no longer borrowed

        var differenceInDays = numberOfDays;

        if (differenceInDays > 14)
        {
            // Calculate the expected total amount based on the difference in days and rental price
            var expectedTotalAmount = book.RentalPrice + (differenceInDays - 14) * (_percentage * book.RentalPrice);

            // Assert that the calculated total amount matches the expected total amount
            Assert.AreEqual(expectedTotalAmount, totalAmount);
        }
        else
        {
            // The difference in days is not greater than 14, so the total amount should be equal to the book's rental price
            Assert.AreEqual(book.RentalPrice, totalAmount);
        }
    }
}
