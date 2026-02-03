using Insurance.Application.Dtos.v1;
using Insurance.Application.Extensions;
using Insurance.Application.Interfaces;
using Insurance.Domain;
using Insurance.Domain.Interfaces;

namespace Insurance.Application;

public class PolicyRenewer(IStorePolicies policyStore, IAmAClock? clock = null) : IRenewPolicies
{
    public Resulting<HouseholdPolicyDto> RenewHouseholdPolicy(HouseholdPolicyDto policy)
    {
        if (policy.UniqueReference is null || policy.UniqueReference == Guid.Empty)
            return Error.Validation("policy.uniqueReference.required", "UniqueReference is required to renew a policy.");

        var fetchResult = policyStore.FetchHouseholdPolicy(policy.UniqueReference.Value);
        if (!fetchResult.IsSuccess)
            return fetchResult.Error ?? Error.NotFound("policy.not.found", "Policy not found.");

        var newAmountResult = Money.Create(policy.Amount);
        if (!newAmountResult.IsSuccess)
            return newAmountResult.Error!;

        if (policy.Payments is null || policy.Payments.Count == 0)
            return Error.Validation("policy.renewal.payment.required", "A renewal payment must be provided.");
        
        if (policy.Payments.Count > 1)
            return Error.Validation("policy.renewal.payment.tooMany", "A single renewal payment must be provided.");

        var renewalPaymentDto = policy.Payments.Single();

        var renewalPaymentResult = PolicyPayment.Create(
            paymentReference: renewalPaymentDto.PaymentReference,
            paymentType: renewalPaymentDto.PaymentType,
            amount: renewalPaymentDto.Amount);

        if (!renewalPaymentResult.IsSuccess)
            return renewalPaymentResult.Error!;

        var domainPolicy = fetchResult.Value;

        var renewResult = domainPolicy.Renew(
            newAmount: newAmountResult.Value,
            renewalPayment: renewalPaymentResult.Value,
            clock: clock);

        if (!renewResult.IsSuccess)
            return renewResult.Error!;

        return Resulting<HouseholdPolicyDto>.Success(domainPolicy.ToDto());
    }

    public Resulting<BuyToLetPolicyDto> RenewBuyToLetPolicy(BuyToLetPolicyDto policy)
    {
        throw new NotImplementedException();
    }
}