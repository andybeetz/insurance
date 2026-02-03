using Insurance.Domain;

namespace Insurance.Api.Dtos.v1;

public record PaymentDto
{
    public required Guid PaymentReference { get; init; }
    public required string PaymentType { get; init; }
    public required decimal Amount { get; init; }
    
    public static PaymentDto FromDomain(PolicyPayment policyPayment)
        => new()
        {
            PaymentReference = policyPayment.PaymentReference,
            PaymentType = policyPayment.PaymentType.Value,
            Amount = policyPayment.Amount.Amount
        };
}