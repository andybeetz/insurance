using Insurance.Domain.Interfaces;

namespace Insurance.Domain;

public class SystemClock : IAmAClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow.Date);
}