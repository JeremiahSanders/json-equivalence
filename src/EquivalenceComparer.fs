namespace Jds.JsonEquivalence

[<AbstractClass; Sealed>]
type EquivalenceComparer private () =
    static member AreEquivalent(comparisonOptions, expected, actual) =
        JsonComparison.compare comparisonOptions expected actual
    static member AreStrictlyEquivalent(expected, actual) =
        JsonComparison.compare EquivalenceOptions.Strict expected actual
