using Insurance.Domain.Helpers;

namespace Insurance.Domain;

public sealed class HouseholdPolicy : Policy
{
    private HouseholdPolicy(
        Guid uniqueReference,
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolders policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
        : base(uniqueReference, period, amount, hasClaims, autoRenew, policyHolders, property, payments) { }

    public static Resulting<HouseholdPolicy> Sell(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolders policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        // Additional HouseholdPolicy validation here
        
        return PolicySelling.SellNew(
            period,
            amount,
            hasClaims,
            autoRenew,
            policyHolders,
            property,
            payments,
            (id, p, a, hc, ar, holders, prop, pay) =>
                new HouseholdPolicy(id, p, a, hc, ar, holders, prop, pay));
    }
}