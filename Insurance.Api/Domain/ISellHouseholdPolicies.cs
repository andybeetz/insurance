namespace Insurance.Api.Domain;

public interface ISellHouseholdPolicies
{
    Resulting<HouseholdPolicy> Sell(HouseholdPolicy policy);
}