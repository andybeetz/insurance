using Insurance.Domain.Interfaces;

namespace Insurance.Domain.Helpers;

internal static class PolicySelling
{
    internal static Resulting<TPolicy> SellNew<TPolicy>(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        IReadOnlyCollection<PolicyHolder> policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments,
        Func<Guid, PolicyPeriod, Money, bool, bool, IReadOnlyCollection<PolicyHolder>, InsuredProperty, IReadOnlyCollection<PolicyPayment>, TPolicy> create,
        IAmAClock? clock = null)
        where TPolicy : Policy
    {
        if (payments is null)
            return Error.Validation("policy.payments.required", "Payments are required.");
        
        clock ??= new SystemClock();
        
        // enforce not selling policies more than 60 days in advance
        var latestAllowedStart = clock.Today.AddDays(60);
        if (period.StartDate > latestAllowedStart)
            return Error.Validation(
                "policy.startDate.tooFarInAdvance",
                "Policies cannot be sold more than 60 days in advance.");

        // Using Guid.NewGuid for unique references, collisions are highly unlikely
        return Resulting<TPolicy>.Success(
            create(Guid.NewGuid(), period, amount, hasClaims, autoRenew, policyHolders, property, payments));
    }
}