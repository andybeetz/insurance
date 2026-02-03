using Insurance.Application.Dtos.v1;
using Insurance.Application.Extensions;
using Insurance.Application.Interfaces;
using Insurance.Domain;

namespace Insurance.Application;

public class PolicyRetriever(IStorePolicies policyStore) : IRetrievePolicies
{
    public Resulting<BuyToLetPolicyDto> RetrieveBuyToLetPolicy(Guid uniqueReference)
    {
        var policyResult = policyStore.FetchBuyToLetPolicy(uniqueReference);

        if (!policyResult.IsSuccess)
            return policyResult.Error ?? Error.Failure("policy.not.found", "Policy not found.");

        return Resulting<BuyToLetPolicyDto>.Success(policyResult.Value.ToDto());
    }

    public Resulting<HouseholdPolicyDto> RetrieveHouseholdPolicy(Guid uniqueReference)
    {
        var policyResult = policyStore.FetchHouseholdPolicy(uniqueReference);

        if (!policyResult.IsSuccess)
            return policyResult.Error ?? Error.Failure("policy.not.found", "Policy not found.");

        return Resulting<HouseholdPolicyDto>.Success(policyResult.Value.ToDto());
    }
}