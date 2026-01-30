using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Insurance.Api.Tests.Integration.Helpers;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly ISellHouseholdPolicies _policySeller;

    public TestWebApplicationFactory(ISellHouseholdPolicies policySeller)
    {
        _policySeller = policySeller;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?> { { "EmailAddress", "test1@Contoso.com" } });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(_policySeller);
        });

        return base.CreateHost(builder);
    }
}