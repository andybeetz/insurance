namespace Insurance.Domain;

public sealed class BuyToLetPolicy : Policy
{
    private BuyToLetPolicy() { }

    public static Resulting<BuyToLetPolicy> Sell(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolder policyHolder,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        if (amount.Amount < 0)
            return Error.Validation("policy.amount.invalid", "Amount must be zero or greater.");

        if (payments is null)
            return Error.Validation("policy.payments.required", "Payments are required.");

        var policy = new BuyToLetPolicy
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

        return Resulting<BuyToLetPolicy>.Success(policy);
    }
}