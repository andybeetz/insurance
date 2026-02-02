namespace Insurance.Domain.Interfaces;

public interface IAmAClock
{
    DateOnly Today { get; }
}