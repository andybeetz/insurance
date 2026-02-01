using Insurance.Api.Dtos.v1;

namespace Insurance.Api;

/// <summary>
/// Abstraction that allows us to use Domain behaviour without cluttering the API endpoints
/// </summary>
public interface ISellBuyToLetPolicies
{
    Resulting<BuyToLetPolicyDto> Sell(BuyToLetPolicyDto policy);
}