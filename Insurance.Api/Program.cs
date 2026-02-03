using Insurance.Api;
using Insurance.Api.Dtos.v1;
using Insurance.Api.Interfaces;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<ISellPolicies, PolicySeller>();
builder.Services.AddTransient<IRetrievePolicies, PolicyRetriever>();
builder.Services.AddTransient<ICancelPolicies, PolicyCanceller>();
builder.Services.AddTransient<IRenewPolicies, PolicyRenewer>();
// Update this registration when you want to use a 'real' data store
builder.Services.AddSingleton<IStorePolicies>(new PolicyStore());

// Add SwaggerGen so we can use the UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Insurance.Api", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("v1/swagger.json", "Insurance.Api v1");
});

app.MapPost("/policies/v1/household",
    (HouseholdPolicyDto policy, ISellPolicies policySeller) =>
    {
        var result = policySeller.SellHouseholdPolicy(policy);

        if (result.IsSuccess)
            return Results.Created($"/policies/v1/household/{policy.UniqueReference}",
                result.Value);
        
        return Results.BadRequest(result.Error);
    }).WithName("SellHouseholdPolicy");

app.MapPost("/policies/v1/buytolet",
    (BuyToLetPolicyDto policy, ISellPolicies policySeller) =>
    {
        var result = policySeller.SellBuyToLetPolicy(policy);

        if (result.IsSuccess)
            return Results.Created($"/policies/v1/buytolet/{policy.UniqueReference}",
                result.Value);
        
        return Results.BadRequest(result.Error);
    }).WithName("SellBuyToLetPolicy");
    
app.MapGet("/policies/v1/household/{uniqueReference:guid}",
    (Guid uniqueReference, IRetrievePolicies policyRetriever) =>
    {
        var result = policyRetriever.RetrieveHouseholdPolicy(uniqueReference);

        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.NotFound(result.Error);
    }).WithName("RetrieveHouseholdPolicy");

app.MapGet("/policies/v1/buytolet/{uniqueReference:guid}",
    (Guid uniqueReference, IRetrievePolicies policyRetriever) =>
    {
        var result = policyRetriever.RetrieveBuyToLetPolicy(uniqueReference);

        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return Results.NotFound(result.Error);
    }).WithName("RetrieveBuyToLetPolicy");

app.MapDelete("/policies/v1/buytolet/{uniqueReference:guid}",
    (Guid uniqueReference, ICancelPolicies policyCanceller) =>
    {
        var result = policyCanceller.CancelBuyToLetPolicy(uniqueReference);

        if (result.IsSuccess)
            return Results.NoContent();

        return Results.BadRequest(result.Error);
    }).WithName("CancelBuyToLetPolicy");

app.MapDelete("/policies/v1/household/{uniqueReference:guid}",
    (Guid uniqueReference, ICancelPolicies policyCanceller) =>
    {
        var result = policyCanceller.CancelHouseholdPolicy(uniqueReference);

        if (result.IsSuccess)
            return Results.NoContent();

        return Results.BadRequest(result.Error);
    }).WithName("CancelHouseholdPolicy");

app.MapPatch("/policies/v1/household",
    (HouseholdPolicyDto policy, IRenewPolicies policyRenewer) =>
    {
        var result = policyRenewer.RenewHouseholdPolicy(policy);

        if (result.IsSuccess)
            return Results.Ok(result.Value);
        
        return Results.BadRequest(result.Error);
    }).WithName("RenewHouseholdPolicy");

app.MapPatch("/policies/v1/buytolet",
    (BuyToLetPolicyDto policy, IRenewPolicies policyRenewer) =>
    {
        var result = policyRenewer.RenewBuyToLetPolicy(policy);

        if (result.IsSuccess)
            return Results.Ok(result.Value);
        
        return Results.BadRequest(result.Error);
    }).WithName("RenewBuyToLetPolicy");

app.MapSwagger();


app.UseHttpsRedirection();

app.Run();