namespace Insurance.Domain;

public sealed record PolicyHolders
{
    private const int MinHolders = 1;
    private const int MaxHolders = 3;

    private PolicyHolders(IReadOnlyCollection<PolicyHolder> value) => Value = value;

    public IReadOnlyCollection<PolicyHolder> Value { get; }

    public static Resulting<PolicyHolders> Create(IReadOnlyCollection<PolicyHolder> policyHolders)
    {
        if (policyHolders is null)
            return Error.Validation("policy.holders.required", "At least one policy holder is required.");

        if (policyHolders.Count < MinHolders)
            return Error.Validation("policy.holders.tooFew", "At least one policy holder is required.");

        if (policyHolders.Count > MaxHolders)
            return Error.Validation("policy.holders.tooMany", "No more than 3 policy holders are allowed.");

        return Resulting<PolicyHolders>.Success(new PolicyHolders(policyHolders));
    }
}