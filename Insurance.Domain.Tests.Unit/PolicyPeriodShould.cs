namespace Insurance.Domain.Tests.Unit;

[TestFixture]
public class PolicyPeriodShould
{
    [Test]
    public void CreateAPeriodOneYearInLength()
    {
        var result = PolicyPeriod.Create(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddYears(1)));
        
        Assert.That(result.IsSuccess);
    }

    [TestCase(1)]
    [TestCase(366)]
    public void FailToCreateAPeriodLessThanOneYearInLength(int days)
    {
        var result = PolicyPeriod.Create(
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.Date),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(days)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error!.Code, Is.EqualTo("policy.period.invalidLength"));
            Assert.That(result.Error!.Description, Is.Not.Null.Or.Empty);
        }
    }
}