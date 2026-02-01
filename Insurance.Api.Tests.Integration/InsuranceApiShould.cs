using System.Net;
using System.Net.Http.Json;
using FakeItEasy;
using Insurance.Api.Dtos.v1;
using Insurance.Api.Tests.Integration.Helpers;

namespace Insurance.Api.Tests.Integration;

[TestFixture]
public class InsuranceApiShould
{
    private TestWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    private ISellHouseholdPolicies _houseHoldPolicySeller;
    private ISellBuyToLetPolicies _buyToLetPolicySeller;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _houseHoldPolicySeller = A.Fake<ISellHouseholdPolicies>();
        _buyToLetPolicySeller = A.Fake<ISellBuyToLetPolicies>();
        _factory = new TestWebApplicationFactory<Program>(_houseHoldPolicySeller, _buyToLetPolicySeller);
        _httpClient = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        Fake.Reset(_houseHoldPolicySeller);
        Fake.Reset(_buyToLetPolicySeller);
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory.Dispose();
        _httpClient.Dispose();
    }

    [TestCase("020B67E9-8430-437B-A45A-F0BDE2061D37")]
    [TestCase("020B67E9-9999-437B-A45A-F0BDE2061D37")]
    public async Task SellAHouseholdPolicy(string policyReference)
    {
        var expectedPolicy = CreateAHouseholdPolicyDto(policyReference);
        var newPolicyRequest = expectedPolicy with { UniqueReference = null };
        
        A.CallTo(() => _houseHoldPolicySeller.Sell(A<HouseholdPolicyDto>._))
            .ReturnsLazily(() => Resulting<HouseholdPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.PostAsJsonAsync("/policies/v1/household", newPolicyRequest);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var soldPolicy = await response.Content.ReadFromJsonAsync<HouseholdPolicyDto>();
            Assert.That(soldPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }
    
    [TestCase("020B67E9-8430-437B-A45A-F0BDE2061D37")]
    [TestCase("020B67E9-9999-437B-A45A-F0BDE2061D37")]
    public async Task SellABuyToLetPolicy(string policyReference)
    {
        var expectedPolicy = CreateABuyToLetPolicyDto(policyReference);
        var newPolicyRequest = expectedPolicy with { UniqueReference = null };
        
        A.CallTo(() => _buyToLetPolicySeller.Sell(A<BuyToLetPolicyDto>._))
            .ReturnsLazily(() => Resulting<BuyToLetPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.PostAsJsonAsync("/policies/v1/buytolet", newPolicyRequest);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var soldPolicy = await response.Content.ReadFromJsonAsync<BuyToLetPolicyDto>();
            Assert.That(soldPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }

    private static HouseholdPolicyDto CreateAHouseholdPolicyDto(string policyReferenceString)
    {
        var startDate = new DateOnly(2021, 05, 01);
        var endDate = startDate.AddDays(365);
        var dateOfBirth = new DateOnly(205, 05, 17);
        var policyReference = new Guid(policyReferenceString);
        var paymentReference = new Guid("120B67E9-8430-437B-A45A-F0BDE2061D38");
        var autoRenew = true;

        var expectedPolicy = new HouseholdPolicyDto
        {
            UniqueReference = policyReference,
            StartDate = startDate,
            EndDate = endDate,
            Amount = 120000m,
            HasClaims = false,
            AutoRenew = autoRenew,
            PolicyHolder = new PolicyHolderDto
            {
                FirstName = "Fred",
                LastName = "Flintstone",
                DateOfBirth = dateOfBirth
            },
            Property = new PropertyDto
            {
                AddressLine1 = "123 Main Street",
                PostCode = "NN12 3GG"
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = paymentReference,
                    PaymentType = "OnlineCard",
                    Amount = 77.11m
                }
            ]
        };
        return expectedPolicy;
    }
    
    private static BuyToLetPolicyDto CreateABuyToLetPolicyDto(string policyReferenceString)
    {
        var startDate = new DateOnly(2021, 05, 01);
        var endDate = startDate.AddDays(365);
        var dateOfBirth = new DateOnly(205, 05, 17);
        var policyReference = new Guid(policyReferenceString);
        var paymentReference = new Guid("120B67E9-8430-437B-A45A-F0BDE2061D38");
        var autoRenew = true;

        var expectedPolicy = new BuyToLetPolicyDto()
        {
            UniqueReference = policyReference,
            StartDate = startDate,
            EndDate = endDate,
            Amount = 120000m,
            HasClaims = false,
            AutoRenew = autoRenew,
            PolicyHolder = new PolicyHolderDto
            {
                FirstName = "Fred",
                LastName = "Flintstone",
                DateOfBirth = dateOfBirth
            },
            Property = new PropertyDto
            {
                AddressLine1 = "123 Main Street",
                PostCode = "NN12 3GG"
            },
            Payments =
            [
                new PaymentDto
                {
                    PaymentReference = paymentReference,
                    PaymentType = "OnlineCard",
                    Amount = 77.11m
                }
            ]
        };
        return expectedPolicy;
    }
}