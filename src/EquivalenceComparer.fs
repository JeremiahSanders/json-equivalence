namespace Jds.JsonEquivalence

///<summary>Equivalence comparer for JSON strings.</summary>
[<AbstractClass; Sealed>]
type EquivalenceComparer private () =

    ///<summary>Compares equivalence between two JSON strings using the provided options.</summary>
    static member AreEquivalent(comparisonOptions, expected, actual) =
        JsonComparison.compare comparisonOptions expected actual

    ///<summary>Compares equivalence between two JSON strings using <see cref="EquivalenceOptions.Strict" />.</summary>
    static member AreStrictlyEquivalent(expected, actual) =
        JsonComparison.compare EquivalenceOptions.Strict expected actual
