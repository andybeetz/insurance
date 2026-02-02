using Insurance.Domain;

namespace Insurance.Api.Dtos.v1;

public record HouseholdPolicyDto : PolicyDto
{
    public override HouseholdPolicy ToDomain() => new()
    {
        UniqueReference = UniqueReference ?? Guid.Empty,
        StartDate = StartDate,
        EndDate = EndDate,
        Amount = Amount,
        HasClaims = HasClaims,
        AutoRenew = AutoRenew,
        PolicyHolder = new PolicyHolder
        {
            FirstName = PolicyHolder.FirstName,
            LastName = PolicyHolder.LastName,
            DateOfBirth = PolicyHolder.DateOfBirth
        },
        Property = new InsuredProperty
        {
            AddressLine1 = Property.AddressLine1,
            AddressLine2 = Property.AddressLine2,
            AddressLine3 = Property.AddressLine3,
            PostCode = Property.PostCode
        },
        Payments = Payments.Select(p => new PolicyPayment
            { PaymentReference = p.PaymentReference, PaymentType = p.PaymentType, Amount = p.Amount }).ToList()
    };
}