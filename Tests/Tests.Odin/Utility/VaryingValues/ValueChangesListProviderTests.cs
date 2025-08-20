using System.Text;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Odin.Utility.VaryingValues;

namespace Tests.Odin.Utility.VaryingValues;

[TestFixture]
public sealed class ValueChangesListProviderTests
{
    [Test]
    [TestCase("1899-12-31", 0.0)]
    [TestCase("1900-01-01", 15.0)]
    [TestCase("2025-04-30", 15.0)]
    [TestCase("2025-05-01", 15.5)]
    [TestCase("2025-05-02", 15.5)]
    [TestCase("2026-03-31", 15.5)]
    [TestCase("2026-04-01", 16.0)]
    public void Vat_changes_in_time_are_correct(string testDateString, decimal? shouldBeValue)
    {
        DateOnly testDate = DateOnly.Parse(testDateString);
        ValueChangesListProvider<DateOnly, decimal> sut =
            new ValueChangesListProvider<DateOnly, decimal>(new List<ValueChange<DateOnly, decimal>>()
            {
                new(new DateOnly(2026, 4, 1), 16.0m),
                new(new DateOnly(1900, 1, 1), 15.0m),
                new(new DateOnly(2025, 5, 1), 15.5m)
            });

        Assert.That(sut, Is.Not.Null);
        var result = sut.GetValue(testDate);
        Assert.That(result, Is.EqualTo(shouldBeValue), $"Expected {shouldBeValue} but was {result}.");
    }

    [Test]
    public void Initialise_from_IConfiguration()
    {
        ValueChangesListProvider<DateOnly, decimal> sut =
            new ValueChangesListProvider<DateOnly, decimal>(CreateTestVaryingDecimalsConfiguration(), "VaryingDecimals");
        
        Assert.That(sut._valueChangesInOrder[0].From, Is.EqualTo(new DateOnly(1900, 1, 1)));
        Assert.That(sut._valueChangesInOrder[0].Value, Is.EqualTo(15.0m));
        Assert.That(sut._valueChangesInOrder[1].From, Is.EqualTo(new DateOnly(2025, 5, 1)));
        Assert.That(sut._valueChangesInOrder[1].Value, Is.EqualTo(15.5m));
    }

    [Test]
    public void Initialise_from_IConfigurationSection()
    {
        ValueChangesListProvider<DateOnly, decimal> sut =
            new ValueChangesListProvider<DateOnly, decimal>(CreateTestVaryingDecimalsConfiguration().GetSection("VaryingDecimals"));
        
        Assert.That(sut._valueChangesInOrder[0].From, Is.EqualTo(new DateOnly(1900, 1, 1)));
        Assert.That(sut._valueChangesInOrder[0].Value, Is.EqualTo(15.0m));
        Assert.That(sut._valueChangesInOrder[1].From, Is.EqualTo(new DateOnly(2025, 5, 1)));
        Assert.That(sut._valueChangesInOrder[1].Value, Is.EqualTo(15.5m));
    }

    private IConfigurationRoot CreateTestVaryingDecimalsConfiguration()
    {
        string json = """
                      {
                        "VaryingDecimals": [
                          {
                            "From": "1900-01-01",
                            "Value": 15
                          },
                          {
                            "From": "2025-05-01",
                            "Value": 15.5
                          }
                        ]
                      }
                      """;
        using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
    }


    private static List<ValueChange<DateOnly, Decimal>> GetValueChangeTestValues()
    {
        return new List<ValueChange<DateOnly, Decimal>>()
        {
            new(new DateOnly(2026, 4, 1), 16.0m),
            new(new DateOnly(1900, 1, 1), 15.0m),
            new(new DateOnly(2025, 5, 1), 15.5m)
        };
    }
}