using Xunit;

namespace Jds.JsonEquivalence.Tests.CSharp
{
  /// <summary>
  ///   Tests validating the <see cref="EquivalenceComparer" /> API, as used by C#.
  /// </summary>
  public static class EquivalenceComparerTests
  {
    private const string MinifiedExample = @"{""name"":""Test""}";

    private const string PrettyPrintedExample = @"{
  ""name"" : ""Test""
}";

    private const string PrettyPrintedDifferingCasingExample = @"{
  ""name"" : ""TEST""
}";

    /// <summary>
    ///   Tests validating the <see cref="EquivalenceComparer.AreEquivalent" /> API, as used by C#.
    /// </summary>
    public class AreEquivalent
    {
      private static bool Act(string expected, string actual)
      {
        var options = new EquivalenceOptions(new EquivalenceValueOptions(false));
        return EquivalenceComparer.AreEquivalent(options, expected, actual);
      }

      [Theory]
      [InlineData(MinifiedExample, PrettyPrintedExample, true)]
      [InlineData(MinifiedExample, PrettyPrintedDifferingCasingExample, true)]
      public void IdentifiesEquivalence(string expected, string actual, bool shouldMatch)
      {
        Assert.Equal(shouldMatch, Act(expected, actual));
      }
    }

    /// <summary>
    ///   Tests validating the <see cref="EquivalenceComparer.AreStrictlyEquivalent" /> API, as used by C#.
    /// </summary>
    public class AreStrictlyEquivalent
    {
      private static bool Act(string expected, string actual)
      {
        return EquivalenceComparer.AreStrictlyEquivalent(expected, actual);
      }

      [Theory]
      [InlineData(MinifiedExample, PrettyPrintedExample, true)]
      [InlineData(MinifiedExample, PrettyPrintedDifferingCasingExample, false)]
      public void IdentifiesEquivalence(string expected, string actual, bool shouldMatch)
      {
        Assert.Equal(shouldMatch, Act(expected, actual));
      }
    }
  }
}
