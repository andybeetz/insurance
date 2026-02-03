using Insurance.Application.Dtos.v1;
using Insurance.Application.Interfaces;
using Insurance.Domain;

namespace Insurance.Application;

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