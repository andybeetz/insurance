using Insurance.Api.Dtos.v1;

namespace Insurance.Api.Interfaces;

/// <summary>
/// Abstraction that allows us to use Domain behaviour without cluttering the API endpoints
/// </summary>
public interface IRetrievePolicies
{
    Resulting<BuyToLetPolicyDto> RetrieveBuyToLetPolicy(Guid uniqueReference);
    Resulting<HouseholdPolicyDto> RetrieveHouseholdPolicy(Guid uniqueReference);
}