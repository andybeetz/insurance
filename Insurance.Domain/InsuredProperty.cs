using System.ComponentModel.DataAnnotations;

namespace Insurance.Domain;

public class InsuredProperty
{
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? AddressLine3 { get; init; }
    [MaxLength(8)]
    public required string PostCode { get; init; }
}