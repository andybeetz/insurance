using Insurance.Api.Domain;
using Insurance.Api.Dtos.v1;

namespace Insurance.Api;

public interface ISellHouseholdPolicies
{
    Resulting<HouseholdPolicy> Sell(HouseholdPolicyDto policy);
}