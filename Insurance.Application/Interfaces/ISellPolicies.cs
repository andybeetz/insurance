using Insurance.Application.Dtos.v1;
using Insurance.Domain;

namespace Insurance.Application.Interfaces;

/// <summary>
/// Abstraction that allows us to use Domain behaviour without cluttering the API endpoints
/// </summary>
public interface ISellPolicies
{
    Resulting<HouseholdPolicyDto> SellHouseholdPolicy(HouseholdPolicyDto policy);
    Resulting<BuyToLetPolicyDto> SellBuyToLetPolicy(BuyToLetPolicyDto policy);
}