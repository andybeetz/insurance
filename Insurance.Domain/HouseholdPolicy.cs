namespace Insurance.Domain;

public sealed class HouseholdPolicy : Policy
{
    private HouseholdPolicy() { }

    public static Resulting<HouseholdPolicy> Sell(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolder policyHolder,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        var policy = new HouseholdPolicy
        {
            UniqueReference = Guid.NewGuid(),
            Period = period,
            Amount = amount,
            HasClaims = hasClaims,
            AutoRenew = autoRenew,
            PolicyHolder = policyHolder,
            Property = property,
            Payments = payments
        };

        return Resulting<HouseholdPolicy>.Success(policy);
    }
}