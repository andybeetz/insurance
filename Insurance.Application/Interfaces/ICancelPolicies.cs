using Insurance.Domain;

namespace Insurance.Application.Interfaces;

/// <summary>
/// Abstraction that allows us to use Domain behaviour without cluttering the API endpoints
/// </summary>
public interface ICancelPolicies
{
    Result CancelHouseholdPolicy(Guid uniqueReference);
    Result CancelBuyToLetPolicy(Guid uniqueReference);
}