using System.Net;
using System.Net.Http.Json;
using FakeItEasy;
using Insurance.Api.Dtos.v1;
using Insurance.Api.Interfaces;
using Insurance.Api.Tests.Integration.Helpers;

namespace Insurance.Api.Tests.Integration;

[TestFixture]
public class InsuranceApiShould
{
    private TestWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    private ISellPolicies _policySeller;
    private IRetrievePolicies _policyRetriever;
    private ICancelPolicies _policyCanceller;
    private IRenewPolicies _policyRenewer;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _policySeller = A.Fake<ISellPolicies>();
        _policyRetriever = A.Fake<IRetrievePolicies>();
        _policyCanceller = A.Fake<ICancelPolicies>();
        _policyRenewer = A.Fake<IRenewPolicies>();
        _factory = new TestWebApplicationFactory<Program>(_policySeller, _policyRetriever, _policyCanceller, _policyRenewer);
        _httpClient = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        Fake.Reset(_policySeller);
        Fake.Reset(_policyRetriever);
        Fake.Reset(_policyCanceller);
        Fake.Reset(_policyRenewer);
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
        
        A.CallTo(() => _policySeller.SellHouseholdPolicy(A<HouseholdPolicyDto>._))
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
        
        A.CallTo(() => _policySeller.SellBuyToLetPolicy(A<BuyToLetPolicyDto>._))
            .ReturnsLazily(() => Resulting<BuyToLetPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.PostAsJsonAsync("/policies/v1/buytolet", newPolicyRequest);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var soldPolicy = await response.Content.ReadFromJsonAsync<BuyToLetPolicyDto>();
            Assert.That(soldPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }

    [Test]
    public async Task RetrieveAHouseholdPolicy()
    {
        var policyReference = Guid.NewGuid();
        var expectedPolicy = CreateAHouseholdPolicyDto(policyReference.ToString());
        
        A.CallTo(() => _policyRetriever.RetrieveHouseholdPolicy(policyReference))
            .Returns(Resulting<HouseholdPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.GetAsync($"/policies/v1/household/{policyReference}");

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var retrievedPolicy = await response.Content.ReadFromJsonAsync<HouseholdPolicyDto>();
            Assert.That(retrievedPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }
    
    [Test]
    public async Task RetrieveABuyToLetPolicy()
    {
        var policyReference = Guid.NewGuid();
        var expectedPolicy = CreateABuyToLetPolicyDto(policyReference.ToString());
        
        A.CallTo(() => _policyRetriever.RetrieveBuyToLetPolicy(policyReference))
            .Returns(Resulting<BuyToLetPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.GetAsync($"/policies/v1/buytolet/{policyReference}");

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var retrievedPolicy = await response.Content.ReadFromJsonAsync<BuyToLetPolicyDto>();
            Assert.That(retrievedPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }
    
    [Test]
    public async Task CancelABuyToLetPolicy()
    {
        var policyReference = Guid.NewGuid();
        
        A.CallTo(() => _policyCanceller.CancelBuyToLetPolicy(policyReference))
            .Returns(Result.Success());

        var response = await _httpClient.DeleteAsync($"/policies/v1/buytolet/{policyReference}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
    
    [Test]
    public async Task CancelAHouseholdPolicy()
    {
        var policyReference = Guid.NewGuid();
        
        A.CallTo(() => _policyCanceller.CancelHouseholdPolicy(policyReference))
            .Returns(Result.Success());

        var response = await _httpClient.DeleteAsync($"/policies/v1/household/{policyReference}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
    
    [Test]
    public async Task RenewAHouseholdPolicy()
    {
        var newPolicy = CreateAHouseholdPolicyDto(Guid.NewGuid().ToString(), false);
        var newPolicyRequest = newPolicy with
        {
            EndDate = newPolicy.EndDate.AddYears(1), Payments =
            [
                new PaymentDto
                {
                    PaymentReference = Guid.NewGuid(),
                    PaymentType = "OnlineCard",
                    Amount = 78.99m
                }
            ]
        };
        
        // The result of the PATCH should be the renewed policy with the new payment added
        var expectedPolicy = newPolicyRequest with { Payments = new[] { newPolicyRequest.Payments.First() }.Concat(newPolicy.Payments).ToArray() };
        
        A.CallTo(() => _policyRenewer.RenewHouseholdPolicy(A<HouseholdPolicyDto>._))
            .ReturnsLazily(() => Resulting<HouseholdPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.PatchAsJsonAsync("/policies/v1/household", newPolicyRequest);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var renewedPolicy = await response.Content.ReadFromJsonAsync<HouseholdPolicyDto>();
            Assert.That(renewedPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }
    
    [Test]
    public async Task RenewABuyToLetPolicy()
    {
        var newPolicy = CreateABuyToLetPolicyDto(Guid.NewGuid().ToString(), false);
        var newPolicyRequest = newPolicy with
        {
            EndDate = newPolicy.EndDate.AddYears(1), Payments =
            [
                new PaymentDto
                {
                    PaymentReference = Guid.NewGuid(),
                    PaymentType = "OnlineCard",
                    Amount = 78.99m
                }
            ]
        };
        
        // The result of the PATCH should be the renewed policy with the new payment added
        var expectedPolicy = newPolicyRequest with { Payments = new[] { newPolicyRequest.Payments.First() }.Concat(newPolicy.Payments).ToArray() };
        
        A.CallTo(() => _policyRenewer.RenewBuyToLetPolicy(A<BuyToLetPolicyDto>._))
            .ReturnsLazily(() => Resulting<BuyToLetPolicyDto>.Success(expectedPolicy));

        var response = await _httpClient.PatchAsJsonAsync("/policies/v1/buytolet", newPolicyRequest);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var renewedPolicy = await response.Content.ReadFromJsonAsync<BuyToLetPolicyDto>();
            Assert.That(renewedPolicy, Is.EqualTo(expectedPolicy).UsingPropertiesComparer());
        });
    }

    private static HouseholdPolicyDto CreateAHouseholdPolicyDto(string policyReferenceString, bool autoRenew = true)
    {
        var startDate = new DateOnly(2021, 05, 01);
        var endDate = startDate.AddDays(365);
        var dateOfBirth = new DateOnly(205, 05, 17);
        var policyReference = new Guid(policyReferenceString);
        var paymentReference = new Guid("120B67E9-8430-437B-A45A-F0BDE2061D38");

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
    
    private static BuyToLetPolicyDto CreateABuyToLetPolicyDto(string policyReferenceString, bool autoRenew = true)
    {
        var startDate = new DateOnly(2021, 05, 01);
        var endDate = startDate.AddDays(365);
        var dateOfBirth = new DateOnly(205, 05, 17);
        var policyReference = new Guid(policyReferenceString);
        var paymentReference = new Guid("120B67E9-8430-437B-A45A-F0BDE2061D38");

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