namespace Insurance.Domain;

public sealed record PolicyHolders
{
    private const int MinHolders = 1;
    private const int MaxHolders = 3;
    private const int MinimumAgeYears = 16;

    private PolicyHolders(IReadOnlyCollection<PolicyHolder> value) => Value = value;

    public IReadOnlyCollection<PolicyHolder> Value { get; }

    public static Resulting<PolicyHolders> Create(
        IReadOnlyCollection<PolicyHolder> policyHolders,
        DateOnly policyStartDate)
    {
        if (policyHolders is null)
            return Error.Validation("policy.holders.required", "At least one policy holder is required.");

        // Enforce minimum and maximum number of policy holders
        if (policyHolders.Count < MinHolders)
            return Error.Validation("policy.holders.tooFew", "At least one policy holder is required.");

        if (policyHolders.Count > MaxHolders)
            return Error.Validation("policy.holders.tooMany", "No more than 3 policy holders are allowed.");

        // Enforce minimum age requirement
        foreach (var holder in policyHolders)
        {
            var age = AgeOn(holder.DateOfBirth, policyStartDate);
            if (age < MinimumAgeYears)
            {
                return Error.Validation(
                    "policy.holders.age.tooYoung",
                    "All policy holders must be at least 16 years old on the start date of the policy.");
            }
        }

        return Resulting<PolicyHolders>.Success(new PolicyHolders(policyHolders));
    }

    private static int AgeOn(DateOnly dateOfBirth, DateOnly onDate)
    {
        var age = onDate.Year - dateOfBirth.Year;

        // If birthday hasn’t occurred yet by onDate, subtract 1.
        if (onDate < dateOfBirth.AddYears(age))
            age--;

        return age;
    }
}