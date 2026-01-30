namespace Insurance.Api.Dtos.v1;

public record PaymentDto
{
    public required Guid PaymentReference { get; init; }
    public required string PaymentType { get; init; }
    public required decimal Amount { get; init; }
}