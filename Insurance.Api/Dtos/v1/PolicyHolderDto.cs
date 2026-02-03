using Insurance.Domain;

namespace Insurance.Api.Dtos.v1;

public record PolicyHolderDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    
    public static PolicyHolderDto FromDomain(PolicyHolder policyHolder)
        => new()
        {
            FirstName = policyHolder.FirstName,
            LastName = policyHolder.LastName,
            DateOfBirth = policyHolder.DateOfBirth
        };
}