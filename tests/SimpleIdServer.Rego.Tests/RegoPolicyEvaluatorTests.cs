
using NUnit.Framework;

namespace SimpleIdServer.Rego.Tests;

public class RegoPolicyEvaluatorTests
{
    private const string PolicyPath = "Policies/admin-policy.rego";
    private string _policyContent;

    [OneTimeSetUp]
    public void Setup()
    {
        _policyContent = File.ReadAllText(PolicyPath);
    }

    [Test]
    public void When_UserIsAdmin_Then_ReturnTrue()
    {
        // Arrange
        var inputJson = new Dictionary<string, object>
        {
            { "user", "admin" }
        };
        // Act
        var result = EvaluatePolicy(_policyContent, inputJson);
        // Assert
        Assert.That(result == "True");
    }

    [Test]
    public void When_UserIsNotAdmin_Then_ReturnFalse()
    {
        // Arrange
        var inputJson = new Dictionary<string, object>
        {
            { "user", "notadmin" }
        };
        // Act
        var result = EvaluatePolicy(_policyContent, inputJson);
        // Assert
        Assert.That(result == "False");
    }

    private static string EvaluatePolicy(string regoText, Dictionary<string, object> inputObj)
    {
        return RegoEvaluator.Evaluate(regoText, inputObj);
    }
}
