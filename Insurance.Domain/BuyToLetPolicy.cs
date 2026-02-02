using Insurance.Domain.Helpers;

namespace Insurance.Domain;

public sealed class BuyToLetPolicy : Policy
{
    private BuyToLetPolicy(
        Guid uniqueReference,
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolders policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    : base(uniqueReference, period, amount, hasClaims, autoRenew, policyHolders, property, payments) { }

    public static Resulting<BuyToLetPolicy> Sell(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolders policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        // Additional BuyToLetPolicy validation here
        
        return PolicySelling.SellNew(
            period,
            amount,
            hasClaims,
            autoRenew,
            policyHolders,
            property,
            payments,
            (id, p, a, hc, ar, holders, prop, pay) =>
                new BuyToLetPolicy(id, p, a, hc, ar, holders, prop, pay));
    }
}