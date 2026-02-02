using Insurance.Api.Dtos.v1;
using Insurance.Domain;

namespace Insurance.Api.Interfaces;

public interface IRenewPolicies
{
    Resulting<HouseholdPolicyDto> RenewHouseholdPolicy(HouseholdPolicyDto policy);
    Resulting<BuyToLetPolicyDto> RenewBuyToLetPolicy(BuyToLetPolicyDto policy);
}