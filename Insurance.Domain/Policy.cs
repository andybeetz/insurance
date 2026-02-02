namespace Insurance.Domain;

public abstract class Policy
{
    protected Policy(
        Guid uniqueReference,
        PolicyPeriod period,
        Money amount,
        bool hasClaims,
        bool autoRenew,
        IReadOnlyCollection<PolicyHolder> policyHolders,
        InsuredProperty property,
        IReadOnlyCollection<PolicyPayment> payments)
    {
        UniqueReference = uniqueReference;
        Period = period;
        Amount = amount;
        HasClaims = hasClaims;
        AutoRenew = autoRenew;
        PolicyHolders = policyHolders;
        Property = property;
        Payments = payments;
    }

    public Guid UniqueReference { get; private set; }
    public PolicyPeriod Period { get; private set; }
    public Money Amount { get; private set; }
    public bool HasClaims { get; private set; }
    public bool AutoRenew { get; private set; }
    public IReadOnlyCollection<PolicyHolder> PolicyHolders { get; private set; }
    public InsuredProperty Property { get; private set; }
    public IReadOnlyCollection<PolicyPayment> Payments { get; private set; }
}