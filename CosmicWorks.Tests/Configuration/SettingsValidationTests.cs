using CosmicWorks.Configuration;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CosmicWorks.Tests.Configuration;

public class SettingsValidationTests
{
    private static IList<ValidationResult> Validate(object instance)
    {
        var ctx = new ValidationContext(instance);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, ctx, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void CosmosSettings_Validates_Correctly()
    {
        var good = new CosmosSettings
        {
            Endpoint = "https://example.cosmos.azure.com",
            Database = "CosmicWorks",
            Container = "Products",
            PartitionKeyPath = "/category_id",
            Key = "fake-key-for-tests" // ← required in your model
        };

        Validate(good).Should().BeEmpty();

        var bad = new CosmosSettings
        {
            Endpoint = "not-a-url",
            Database = string.Empty,
            Container = string.Empty,
            PartitionKeyPath = "category_id", // missing '/'
            Key = null
        };

        var results = Validate(bad);
        results.Should().NotBeEmpty();

        // Assert by member name instead of brittle message text
        results.Any(r => r.MemberNames.Contains(nameof(CosmosSettings.PartitionKeyPath))).Should().BeTrue();
        results.Any(r => r.MemberNames.Contains(nameof(CosmosSettings.Key))).Should().BeTrue();
    }

    [Fact]
    public void RagSettings_Enforce_Ranges()
    {
        var ok = new RagSettings { TopK = 3, MinSimilarity = 0.25 };
        Validate(ok).Should().BeEmpty();

        var tooLow = new RagSettings { TopK = 0, MinSimilarity = -0.1 };
        Validate(tooLow).Should().NotBeEmpty();

        var tooHigh = new RagSettings { TopK = 3, MinSimilarity = 1.1 };
        Validate(tooHigh).Should().NotBeEmpty();
    }

    [Fact]
    public void DiscountPolicySettings_Defaults_Are_Sensible()
    {
        var s = new DiscountPolicySettings();
        s.DefaultCap.Should().BeGreaterThan(0.0);
        s.DefaultCap.Should().BeLessThanOrEqualTo(1.0);
        s.Caps.Should().NotBeNull();
    }
}