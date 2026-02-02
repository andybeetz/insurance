using Insurance.Api.Dtos.v1;
using Insurance.Api.Interfaces;
using Insurance.Domain;

namespace Insurance.Api;

public class PolicySeller : ISellPolicies
{
    public Resulting<HouseholdPolicyDto> SellHouseholdPolicy(HouseholdPolicyDto policy)
    {
        throw new NotImplementedException();
    }

    public Resulting<BuyToLetPolicyDto> SellBuyToLetPolicy(BuyToLetPolicyDto policy)
    {
        throw new NotImplementedException();
    }
}