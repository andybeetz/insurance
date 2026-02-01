using Insurance.Api.Domain;
using Insurance.Api.Dtos.v1;

namespace Insurance.Api;

public interface ISellBuyToLetPolicies
{
    Resulting<BuyToLetPolicy> Sell(BuyToLetPolicyDto policy);
}