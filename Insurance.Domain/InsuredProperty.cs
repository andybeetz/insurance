namespace Insurance.Domain;

public sealed record InsuredProperty
{
    private InsuredProperty(string addressLine1, string? addressLine2, string? addressLine3, PostCode postCode)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        PostCode = postCode;
    }

    public string AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? AddressLine3 { get; }
    public PostCode PostCode { get; }

    public static Resulting<InsuredProperty> Create(
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string postCode)
    {
        if (string.IsNullOrWhiteSpace(addressLine1))
            return Error.Validation("property.addressLine1.required", "AddressLine1 is required.");

        var postCodeResult = PostCode.Create(postCode);
        if (!postCodeResult.IsSuccess)
            return postCodeResult.Error!;

        return Resulting<InsuredProperty>.Success(
            new InsuredProperty(addressLine1.Trim(), addressLine2?.Trim(), addressLine3?.Trim(), postCodeResult.Value));
    }
}