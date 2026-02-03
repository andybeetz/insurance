using Insurance.Api.Dtos.v1;
using Insurance.Api.Interfaces;
using Insurance.Domain;

namespace Insurance.Api;

public class PolicyRetriever(IStorePolicies policyStore) : IRetrievePolicies
{
    public Resulting<BuyToLetPolicyDto> RetrieveBuyToLetPolicy(Guid uniqueReference)
    {
        var policyResult = policyStore.FetchBuyToLetPolicy(uniqueReference);

        if (!policyResult.IsSuccess)
            return policyResult.Error ?? Error.Failure("policy.not.found", "Policy not found.");

        return Resulting<BuyToLetPolicyDto>.Success(BuyToLetPolicyDto.FromDomain(policyResult.Value));
    }

    public Resulting<HouseholdPolicyDto> RetrieveHouseholdPolicy(Guid uniqueReference)
    {
        var policyResult = policyStore.FetchHouseholdPolicy(uniqueReference);

        if (!policyResult.IsSuccess)
            return policyResult.Error ?? Error.Failure("policy.not.found", "Policy not found.");

        return Resulting<HouseholdPolicyDto>.Success(HouseholdPolicyDto.FromDomain(policyResult.Value));
    }
}