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
        PolicyHolder policyHolder,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    : base(uniqueReference, period, amount, hasClaims, autoRenew, policyHolder, property, payments) { }

    public static Resulting<BuyToLetPolicy> Sell(
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        PolicyHolder policyHolder,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        // Additional BuyToLetPolicy validation here
        
        return PolicySelling.SellNew(
            period,
            amount,
            hasClaims,
            autoRenew,
            policyHolder,
            property,
            payments,
            (id, p, a, hc, ar, holder, prop, pay) =>
                new BuyToLetPolicy(id, p, a, hc, ar, holder, prop, pay));
    }
}