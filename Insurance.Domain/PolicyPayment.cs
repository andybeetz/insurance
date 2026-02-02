namespace Insurance.Domain;

public sealed record PolicyPayment
{
    private PolicyPayment(Guid paymentReference, string paymentType, decimal amount)
    {
        PaymentReference = paymentReference;
        PaymentType = paymentType;
        Amount = amount;
    }

    public Guid PaymentReference { get; }
    public string PaymentType { get; }
    public decimal Amount { get; }

    public static Resulting<PolicyPayment> Create(Guid paymentReference, string paymentType, decimal amount)
    {
        if (paymentReference == Guid.Empty)
            return Error.Validation("payment.reference.required", "PaymentReference is required.");

        if (string.IsNullOrWhiteSpace(paymentType))
            return Error.Validation("payment.type.required", "PaymentType is required.");

        if (amount < 0)
            return Error.Validation("payment.amount.invalid", "Payment amount must be zero or greater.");

        return Resulting<PolicyPayment>.Success(new PolicyPayment(paymentReference, paymentType.Trim(), amount));
    }
}