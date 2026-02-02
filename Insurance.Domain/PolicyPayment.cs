namespace Insurance.Domain;

public sealed record PolicyPayment
{
    private PolicyPayment(Guid paymentReference, PaymentType paymentType, Money amount)
    {
        PaymentReference = paymentReference;
        PaymentType = paymentType;
        Amount = amount;
    }

    public Guid PaymentReference { get; }
    public PaymentType PaymentType { get; }
    public Money Amount { get; }

    public static Resulting<PolicyPayment> Create(Guid paymentReference, string paymentType, decimal amount)
    {
        if (paymentReference == Guid.Empty)
            return Error.Validation("payment.reference.required", "PaymentReference is required.");

        var paymentTypeResult = PaymentType.Create(paymentType);
        if (!paymentTypeResult.IsSuccess)
            return paymentTypeResult.Error!;

        var moneyResult = Money.Create(amount);
        if (!moneyResult.IsSuccess)
            return moneyResult.Error!;

        return Resulting<PolicyPayment>.Success(
            new PolicyPayment(paymentReference, paymentTypeResult.Value, moneyResult.Value));
    }
}