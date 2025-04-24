using System.Text;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Odin.Utility.Tax;
using Odin.Utility.VaryingValues;

namespace Tests.Odin.Utility.Tax
{
    [TestFixture]
    public class TaxUtilityTests
    {
        [Test]
        [TestCase(115, 15,2, "15", Description = "15% rate")]
        [TestCase(130, 30,2, "30", Description = "30% rate")]
        [TestCase(105, 5,2, "5", Description = "5% rate")]
        [TestCase(95, -5,2, "-5", Description = "Negative rate")]
        [TestCase(-115, 15,10, "-15", Description = "Negative amount")]
        [TestCase(123.78, 15,4, "16.1452", Description = "Decimal places 4")]
        [TestCase(123.78, 15,2, "16.14", Description = "Decimal places 2")]
        [TestCase(123.78, 15,0, "16.00", Description = "Decimal places 0")]
        [TestCase(123.78, 15,-2, "16.00", Description = "Decimal places negative behaves like 0")]
        [TestCase(123.78, 15,25, "16.14521739130434782608", Description = "Decimal places too high > 20")]
        [TestCase(123.78, 15,20, "16.14521739130434782608", Description = "Decimal places 20")]
        [TestCase(123.78, 15,10, "16.1452173913", Description = "Decimal places 10")]
        [TestCase(123.78, 15,null, "16.1452173913", Description = "Default decimal places is 10")]
        public void Calculation_of_an_included_tax_portion_is_correct(decimal amount, decimal taxRatePercentage, int? roundToDecimalPlaces, string expectedAsString)
        {
            decimal expected = decimal.Parse(expectedAsString); // NUnit conversion to decimal only goes up to about 14 decimal points for some reason...
            TaxUtility sut = new TaxUtility(taxRatePercentage);
            decimal result;
            if (roundToDecimalPlaces.HasValue)
            {
                result = sut.CalculateTaxPortionOfTaxInclusiveAmount(amount,DateOnly.MaxValue,roundToDecimalPlaces.Value );
            }
            else
            {
                result = sut.CalculateTaxPortionOfTaxInclusiveAmount(amount,DateOnly.MaxValue);
            }
            Assert.That(result, Is.EqualTo(expected), $"Actual - {result}. Expected - {expected}");
        }

        [Test]
        [TestCase(100, 15,2, 15, Description = "15% rate")]
        [TestCase(100, 30,2, 30, Description = "30% rate")]
        [TestCase(100, 5,2, 5, Description = "5% rate")]
        [TestCase(100, -5,2, -5, Description = "Negative rate")]
        [TestCase(-100, 15,10, -15, Description = "Negative amount")]
        [TestCase(99.37, 15.3,5, 15.20361, Description = "Decimal places 5")]
        [TestCase(99.37, 15.3,25, 15.20361, Description = "Decimal places too high > 20")]
        [TestCase(99.37, 15.3,10, 15.2036100000, Description = "Decimal places 10")]
        [TestCase(99.37, 15.3,4, 15.2036, Description = "Decimal places 5")]
        [TestCase(99.37, 15.3,2, 15.20, Description = "Decimal places 5")]
        [TestCase(99.37, 15.3,0, 15.0, Description = "Decimal places 0")]
        [TestCase(99.37, 15.3,null, 15.2036100000, Description = "Default decimal places is 10")]
        public void Calculation_of_tax_on_an_exclusive_amount_is_correct(decimal amount, decimal taxRatePercentage, int? roundToDecimalPlaces, decimal expected)
        {
            TaxUtility sut = new TaxUtility(taxRatePercentage);
            decimal result;
            if (roundToDecimalPlaces.HasValue)
            {
                result = sut.CalculateTaxOnTaxExclusiveAmount(amount,DateOnly.MaxValue,roundToDecimalPlaces.Value );
            }
            else
            {
                result = sut.CalculateTaxOnTaxExclusiveAmount(amount,DateOnly.MaxValue);
            }
            Assert.That(result, Is.EqualTo(expected), $"Actual - {result}. Expected - {expected}");
        }


        [Test]
        [TestCase(15)]
        [TestCase(15.5)]
        [TestCase(0)]
        public void Using_a_single_tax_rate(decimal testRatePercentage)
        {
            TaxUtility sut = new TaxUtility(testRatePercentage);
            
            Assert.That(sut.GetTaxRateAsFraction(DateOnly.MinValue), Is.EqualTo(testRatePercentage*0.01m));
            Assert.That(sut.GetTaxRateAsPercentage(DateOnly.MinValue), Is.EqualTo(testRatePercentage));
            Assert.That(sut.GetTaxRateAsFraction(DateOnly.MaxValue), Is.EqualTo(testRatePercentage*0.01m));
            Assert.That(sut.GetTaxRateAsPercentage(DateOnly.MaxValue), Is.EqualTo(testRatePercentage));
            Assert.That(sut.GetTaxRateAsFraction(DateOnly.FromDateTime(DateTime.Now)), Is.EqualTo(testRatePercentage*0.01m));
            Assert.That(sut.GetTaxRateAsPercentage(DateOnly.FromDateTime(DateTime.Now)), Is.EqualTo(testRatePercentage));
        }
        
        [Test]
        [TestCase(15)]
        [TestCase(15.5)]
        [TestCase(0)]
        public void Multiple_tax_rates_are_supported(decimal testRatePercentage)
        {
            TaxUtility sut = new TaxUtility(CreateSouthAfricanVatHistory());

            AssertSouthAfricanVatHistoryRates(sut);
        }

        private void AssertSouthAfricanVatHistoryRates(TaxUtility sut)
        {
            Assert.That(sut.GetTaxRateAsPercentage(DateOnly.MinValue), Is.EqualTo(default(decimal)));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(1800, 1, 1)), Is.EqualTo(default(decimal)));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(1900, 1, 1)), Is.EqualTo(15m));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(1900, 1, 2)), Is.EqualTo(15m));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(2025, 4, 30)), Is.EqualTo(15m));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(2025, 5, 1)), Is.EqualTo(15.5m));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(2026, 4, 30)), Is.EqualTo(15.5m));
            Assert.That(sut.GetTaxRateAsPercentage(new DateOnly(2026, 5, 1)), Is.EqualTo(16m));
            Assert.That(sut.GetTaxRateAsPercentage(DateOnly.MaxValue), Is.EqualTo(16m));
        }
        
        
        [Test]
        public void Multiple_tax_rates_are_supported_via_IConfiguration()
        {
            TaxUtility sut = new TaxUtility(CreateSouthAfricanVatHistoryConfiguration(),"TaxHistory");

            AssertSouthAfricanVatHistoryRates(sut);
        }

        [Test]
        public void Multiple_tax_rates_are_supported_via_IConfigurationSection()
        {
            TaxUtility sut = new TaxUtility(CreateSouthAfricanVatHistoryConfiguration().GetSection("TaxHistory"));

            AssertSouthAfricanVatHistoryRates(sut);
        }

        private List<ValueChange<DateOnly, decimal>> CreateSouthAfricanVatHistory()
        {
            return new List<ValueChange<DateOnly, decimal>>()
            {
                new ValueChange<DateOnly, decimal>(new DateOnly(1900, 1, 1), 15m),
                new ValueChange<DateOnly, decimal>(new DateOnly(2025, 5, 1), 15.5m),
                new ValueChange<DateOnly, decimal>(new DateOnly(2026, 5, 1), 16.0m)
            };
        }
        private IConfigurationRoot CreateSouthAfricanVatHistoryConfiguration()
        {
            string json = """
                          {
                            "TaxHistory": [
                              {
                                "From": "1900-01-01",
                                "Value": 15
                              },
                              {
                                "From": "2025-05-01",
                                "Value": 15.5
                              },
                              {
                                "From": "2026-05-01",
                                "Value": 16
                              }
                            ]
                          }
                          """;
            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

    }
}