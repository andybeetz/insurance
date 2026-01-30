using Insurance.Api.Domain;

namespace Insurance.Api.Dtos.v1;

public record HouseholdPolicyDto
{
    public Guid? UniqueReference { get; set; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required decimal Amount { get; init; }
    public required bool HasClaims { get; init; }
    public required bool AutoRenew { get; init; }
    public required PolicyHolderDto PolicyHolder { get; init; }
    public required PropertyDto Property { get; init; }
    public required IReadOnlyCollection<PaymentDto> Payments { get; init; }

    public HouseholdPolicy ToDomain() => new()
    {
        UniqueReference = UniqueReference ?? Guid.Empty,
        StartDate = StartDate,
        EndDate = EndDate,
        Amount = Amount,
        HasClaims = HasClaims,
        AutoRenew = AutoRenew,
        PolicyHolder = new PolicyHolder()
        {
            FirstName = PolicyHolder.FirstName,
            LastName = PolicyHolder.LastName,
            DateOfBirth = PolicyHolder.DateOfBirth
        },
        Property = new InsuredProperty()
        {
            AddressLine1 = Property.AddressLine1,
            AddressLine2 = Property.AddressLine2,
            AddressLine3 = Property.AddressLine3,
            PostCode = Property.PostCode
        },
        Payments = Payments.Select(p => new PolicyPayment()
            { PaymentReference = p.PaymentReference, PaymentType = p.PaymentType, Amount = p.Amount }).ToList()
    };
}