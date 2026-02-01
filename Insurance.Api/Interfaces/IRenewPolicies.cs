using Insurance.Api.Dtos.v1;

namespace Insurance.Api.Interfaces;

public interface IRenewPolicies
{
    Resulting<HouseholdPolicyDto> RenewHouseholdPolicy(HouseholdPolicyDto policy);
}