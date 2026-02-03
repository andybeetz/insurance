namespace Insurance.Application.Dtos.v1;

public record PolicyHolderDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
}