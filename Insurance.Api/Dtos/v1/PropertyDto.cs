using System.ComponentModel.DataAnnotations;
using Insurance.Domain;

namespace Insurance.Api.Dtos.v1;

public record PropertyDto
{
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? AddressLine3 { get; init; }
    [MaxLength(8)]
    public required string PostCode { get; init; }
    
    public static PropertyDto FromDomain(InsuredProperty property)
        => new()
        { 
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            AddressLine3 = property.AddressLine3,
            PostCode = property.PostCode.Value
        };
}