namespace Insurance.Domain.Helpers;

internal static class PolicySelling
{
    internal static Resulting<TPolicy> SellNew<TPolicy>(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolder policyHolder,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments,
        Func<Guid, PolicyPeriod, Money, bool, bool, PolicyHolder, InsuredProperty, IReadOnlyCollection<PolicyPayment>, TPolicy> create)
        where TPolicy : Policy
    {
        if (payments is null)
            return Error.Validation("policy.payments.required", "Payments are required.");

        // Money is a value object; if you enforce non-negative in Money.Create,
        // you can remove amount checks from policies entirely.
        return Resulting<TPolicy>.Success(
            create(Guid.NewGuid(), period, amount, hasClaims, autoRenew, policyHolder, property, payments));
    }
}