namespace Library.Interfaces;

public interface IFees
{
    double CalculateTotalAmount(DateTime borrowDate, double bookPrice);
}
