namespace Insurance.Domain;

public sealed record PaymentType
{
    private PaymentType(string value) => Value = value;

    public string Value { get; }

    public static Resulting<PaymentType> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("payment.type.required", "Payment type is required.");

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized is not ("CARD" or "DIRECTDEBIT" or "CHEQUE"))
            return Error.Validation("payment.type.invalid", "Unsupported payment type.");

        return Resulting<PaymentType>.Success(new PaymentType(normalized));
    }

    public override string ToString() => Value;
}