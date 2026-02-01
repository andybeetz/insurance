namespace Insurance.Api;

public interface ICancelHouseholdPolicies
{
    Result Cancel(Guid uniqueReference);
}