namespace Insurance.Domain;

public sealed record InsuredProperty
{
    private InsuredProperty(string addressLine1, string? addressLine2, string? addressLine3, string postCode)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        PostCode = postCode;
    }

    public string AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? AddressLine3 { get; }
    public string PostCode { get; }

    public static Resulting<InsuredProperty> Create(
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string postCode)
    {
        if (string.IsNullOrWhiteSpace(addressLine1))
            return Error.Validation("property.addressLine1.required", "AddressLine1 is required.");

        if (string.IsNullOrWhiteSpace(postCode))
            return Error.Validation("property.postCode.required", "PostCode is required.");

        var normalizedPostCode = postCode.Trim().ToUpperInvariant();

        // Keep/adjust rule (you previously used MaxLength(8))
        if (normalizedPostCode.Length > 8)
            return Error.Validation("property.postCode.invalid", "PostCode must be 8 characters or fewer.");

        return Resulting<InsuredProperty>.Success(
            new InsuredProperty(addressLine1.Trim(), addressLine2?.Trim(), addressLine3?.Trim(), normalizedPostCode));
    }
}