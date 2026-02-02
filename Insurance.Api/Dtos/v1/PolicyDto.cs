namespace Insurance.Api.Dtos.v1;

public abstract record PolicyDto
{
    public Guid? UniqueReference { get; set; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required decimal Amount { get; init; }
    public required bool HasClaims { get; init; }
    public required bool AutoRenew { get; init; }
    public required PolicyHolderDto PolicyHolder { get; init; }
    public required PropertyDto Property { get; init; }
    public required IReadOnlyCollection<PaymentDto> Payments { get; init; }
}