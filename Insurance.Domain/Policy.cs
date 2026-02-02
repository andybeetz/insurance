namespace Insurance.Domain;

public abstract class Policy
{
    public required Guid UniqueReference { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required Money Amount { get; init; }
    public required bool HasClaims { get; init; }
    public required bool AutoRenew { get; init; }
    public required PolicyHolder PolicyHolder { get; init; }
    public required InsuredProperty Property { get; init; }
    public required IReadOnlyCollection<PolicyPayment> Payments { get; init; }
}