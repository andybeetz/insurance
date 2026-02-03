using Insurance.Application.Dtos.v1;
using Insurance.Domain;

namespace Insurance.Application.Interfaces;

/// <summary>
/// Abstraction that allows us to use Domain behaviour without cluttering the API endpoints
/// </summary>
public interface IRetrievePolicies
{
    Resulting<BuyToLetPolicyDto> RetrieveBuyToLetPolicy(Guid uniqueReference);
    Resulting<HouseholdPolicyDto> RetrieveHouseholdPolicy(Guid uniqueReference);
}