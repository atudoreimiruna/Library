using Library.Interfaces;

namespace Library.Services;

public class FeesService : IFees
{
    public IDateTimeProvider DateTimeProvider { get; set; } = new DateTimeProvider();
    private readonly double _percentage = 0.01;

    public double CalculateTotalAmount(DateTime borrowDate, double bookPrice)
    {
        double totalAmount = 0.0;
        var currentDate = DateTimeProvider.Now; 

        var differenceInDays = (currentDate - borrowDate).Days;
        Console.WriteLine($"The book copy has been borrowed for {differenceInDays} days.");

        if (differenceInDays > 14)
        {
            totalAmount = bookPrice + (differenceInDays - 14) * (_percentage * bookPrice);
            Console.WriteLine($"You passed the limit of 14 days, with {differenceInDays - 14} more days, so you have to pay {totalAmount}.");
        }
        else
        {
            totalAmount = bookPrice;
            Console.WriteLine($"You have to pay {bookPrice}.");
        }
        return totalAmount;
    }
}
