namespace Insurance.Api.Domain;

public interface ISellBuyToLetPolicies
{
    Resulting<BuyToLetPolicy> Sell(BuyToLetPolicy policy);
}