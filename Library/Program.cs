using Library.Models;
using Library.Services;

class Program
{
    static void Main(string[] args)
    {
        BookcaseService manager = new();

        while (true)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. Add a new book");
            Console.WriteLine("2. List all available books");
            Console.WriteLine("3. List all available copies of a book");
            Console.WriteLine("4. Borrow a book");
            Console.WriteLine("5. Return a book");
            Console.WriteLine("6. Quit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter the title: ");
                    string title = Console.ReadLine();
                    Console.Write("Enter the ISBN: ");
                    string isbn = Console.ReadLine();
                    Console.Write("Enter the rental price: ");
                    double rentalPrice = Convert.ToDouble(Console.ReadLine());
                    Console.Write("Enter the number of copies for the book: ");
                    int bookCopies = Convert.ToInt16(Console.ReadLine());

                    Book newBook = new Book
                    {
                        Title = title,
                        ISBN = isbn,
                        RentalPrice = rentalPrice,
                    };

                    manager.AddBook(newBook, bookCopies);
                    break;
                case "2":
                    manager.GetAllBooks();
                    break;
                case "3":
                    Console.Write("Enter the ISBN of the book: ");
                    isbn = Console.ReadLine();
                    manager.GetAllBookCopies(isbn);
                    break;
                case "4":
                    Console.Write("Enter the ISBN of the book you want to borrow: ");
                    isbn = Console.ReadLine();
                    manager.BorrowBook(isbn);
                    break;
                case "5":
                    Console.Write("Enter the id of the book copy you want to return: ");
                    if (!Guid.TryParse(Console.ReadLine(), out var id))
                    {
                        Console.WriteLine("Invalid id");
                        break;
                    }
                    manager.ReturnBook(id);
                    break;
                case "6":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}