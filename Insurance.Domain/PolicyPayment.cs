namespace Insurance.Domain;

public sealed record PolicyPayment
{
    private PolicyPayment(Guid paymentReference, string paymentType, Money amount)
    {
        PaymentReference = paymentReference;
        PaymentType = paymentType;
        Amount = amount;
    }

    public Guid PaymentReference { get; }
    public string PaymentType { get; }
    public Money Amount { get; }

    public static Resulting<PolicyPayment> Create(Guid paymentReference, string paymentType, decimal amount)
    {
        if (paymentReference == Guid.Empty)
            return Error.Validation("payment.reference.required", "PaymentReference is required.");

        if (string.IsNullOrWhiteSpace(paymentType))
            return Error.Validation("payment.type.required", "PaymentType is required.");

        var moneyResult = Money.Create(amount);
        if (!moneyResult.IsSuccess)
            return moneyResult.Error!;

        return Resulting<PolicyPayment>.Success(new PolicyPayment(paymentReference, paymentType.Trim(), moneyResult.Value));
    }
}