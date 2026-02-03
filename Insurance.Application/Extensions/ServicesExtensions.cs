using Insurance.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Application.Extensions;

public static class ServicesExtensions
{
    public static void AddInsuranceApplication(this IServiceCollection services)
    {
        services.AddTransient<ISellPolicies, PolicySeller>();
        services.AddTransient<IRetrievePolicies, PolicyRetriever>();
        services.AddTransient<ICancelPolicies, PolicyCanceller>();
        services.AddTransient<IRenewPolicies, PolicyRenewer>();
        // Update this registration when you want to use a 'real' data store
        services.AddSingleton<IStorePolicies>(new PolicyStore());
    }
}