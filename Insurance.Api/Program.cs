using Insurance.Api.Dtos.v1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/policies/v1/household",
    (HouseholdPolicyDto policy) => Results.Created($"policies/v1/household/{policy.UniqueReference}",
        policy with { UniqueReference = new Guid("020B67E9-8430-437B-A45A-F0BDE2061D37")})).WithName("SellHouseholdPolicy");

app.UseHttpsRedirection();

app.Run();