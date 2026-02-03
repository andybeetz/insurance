using Insurance.Domain;

namespace Insurance.Api.Dtos.v1;

public record HouseholdPolicyDto : PolicyDto
{
    public static HouseholdPolicyDto FromDomain(HouseholdPolicy policy)
        => new()
        {
            UniqueReference = policy.UniqueReference,
            StartDate = policy.Period.StartDate,
            EndDate = policy.Period.EndDate,
            Amount = policy.Amount.Amount,
            HasClaims = policy.HasClaims,
            AutoRenew = policy.AutoRenew,
            PolicyHolders = policy.PolicyHolders.Value.Select(PolicyHolderDto.FromDomain).ToArray(),
            Property = PropertyDto.FromDomain(policy.Property),
            Payments = policy.Payments.Select(PaymentDto.FromDomain).ToArray(),
        };
}