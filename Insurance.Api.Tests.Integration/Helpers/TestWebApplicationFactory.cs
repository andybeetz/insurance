using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Insurance.Api.Tests.Integration.Helpers;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly ISellHouseholdPolicies _householdPolicySeller;
    private readonly ISellBuyToLetPolicies _buyToLetPolicySeller;

    public TestWebApplicationFactory(ISellHouseholdPolicies householdPolicySeller, ISellBuyToLetPolicies buyToLetPolicySeller)
    {
        _householdPolicySeller = householdPolicySeller;
        _buyToLetPolicySeller = buyToLetPolicySeller;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?> { { "EmailAddress", "test1@Contoso.com" } });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(_householdPolicySeller);
            services.AddSingleton(_buyToLetPolicySeller);
        });

        return base.CreateHost(builder);
    }
}