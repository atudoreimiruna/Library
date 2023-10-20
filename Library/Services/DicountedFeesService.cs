using Library.Interfaces;

namespace Library.Services;

public class DicountedFeesService : IFees
{
    public double CalculateTotalAmount(DateTime borrowDate, double bookPrice)
    {
        return 1;
    }
}
