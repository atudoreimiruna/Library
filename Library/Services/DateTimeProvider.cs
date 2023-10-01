using Library.Interfaces;

namespace Library.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}