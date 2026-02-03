using Insurance.Application.Dtos.v1;
using Insurance.Domain;

namespace Insurance.Application.Interfaces;

public interface IRenewPolicies
{
    Resulting<HouseholdPolicyDto> RenewHouseholdPolicy(HouseholdPolicyDto policy);
    Resulting<BuyToLetPolicyDto> RenewBuyToLetPolicy(BuyToLetPolicyDto policy);
}