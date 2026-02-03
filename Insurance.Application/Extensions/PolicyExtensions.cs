using Insurance.Application.Dtos.v1;
using Insurance.Domain;

namespace Insurance.Application.Extensions;

public static class PolicyExtensions
{
    public static BuyToLetPolicyDto ToDto(this BuyToLetPolicy policy)
        => new()
        {
            UniqueReference = policy.UniqueReference,
            StartDate = policy.Period.StartDate,
            EndDate = policy.Period.EndDate,
            Amount = policy.Amount.Amount,
            HasClaims = policy.HasClaims,
            AutoRenew = policy.AutoRenew,
            PolicyHolders = policy.PolicyHolders.Value.Select(p => p.ToDto()).ToArray(),
            Property = policy.Property.ToDto(),
            Payments = policy.Payments.Select(p => p.ToDto()).ToArray(),
        };
    
    public static HouseholdPolicyDto ToDto(this HouseholdPolicy policy)
        => new()
        {
            UniqueReference = policy.UniqueReference,
            StartDate = policy.Period.StartDate,
            EndDate = policy.Period.EndDate,
            Amount = policy.Amount.Amount,
            HasClaims = policy.HasClaims,
            AutoRenew = policy.AutoRenew,
            PolicyHolders = policy.PolicyHolders.Value.Select(p => p.ToDto()).ToArray(),
            Property = policy.Property.ToDto(),
            Payments = policy.Payments.Select(p => p.ToDto()).ToArray(),
        };

    private static PaymentDto ToDto(this PolicyPayment policyPayment)
        => new()
        {
            PaymentReference = policyPayment.PaymentReference,
            PaymentType = policyPayment.PaymentType.Value,
            Amount = policyPayment.Amount.Amount
        };
    
    private static PolicyHolderDto ToDto(this PolicyHolder policyHolder)
        => new()
        {
            FirstName = policyHolder.FirstName,
            LastName = policyHolder.LastName,
            DateOfBirth = policyHolder.DateOfBirth
        };
    
    private static PropertyDto ToDto(this InsuredProperty property)
        => new()
        { 
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            AddressLine3 = property.AddressLine3,
            PostCode = property.PostCode.Value
        };
}