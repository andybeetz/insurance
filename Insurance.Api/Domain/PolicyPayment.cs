namespace Insurance.Api.Domain;

public class PolicyPayment
{
    public required Guid PaymentReference { get; init; }
    public required string PaymentType { get; init; }
    public required decimal Amount { get; init; }
}