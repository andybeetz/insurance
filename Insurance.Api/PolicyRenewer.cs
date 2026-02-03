using Insurance.Api.Dtos.v1;
using Insurance.Api.Interfaces;
using Insurance.Domain;

namespace Insurance.Api;

public class PolicyRenewer : IRenewPolicies
{
    public Resulting<HouseholdPolicyDto> RenewHouseholdPolicy(HouseholdPolicyDto policy)
    {
        throw new NotImplementedException();
    }

    public Resulting<BuyToLetPolicyDto> RenewBuyToLetPolicy(BuyToLetPolicyDto policy)
    {
        throw new NotImplementedException();
    }
}