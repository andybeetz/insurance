using System.ComponentModel.DataAnnotations;

namespace Insurance.Application.Dtos.v1;

public record PropertyDto
{
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? AddressLine3 { get; init; }
    [MaxLength(8)]
    public required string PostCode { get; init; }
}