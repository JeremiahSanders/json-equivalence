namespace Jds.JsonEquivalence.Tests.Unit

open Jds.JsonEquivalence
open Xunit

module EquivalenceComparerTests =
    module AreEquivalent =
        [<Fact>]
        let ``Identifies equivalence``() =
            let options = EquivalenceOptions(EquivalenceValueOptions(false))
            let actual =
                EquivalenceComparer.AreEquivalent
                    (options, TestData.NestedObjectCases.sourceJson, TestData.NestedObjectCases.childPropertyReordered)
            Assert.True(actual)

    module AreStrictlyEquivalent =
        [<Fact>]
        let ``Identifies equivalence``() =
            let actual =
                EquivalenceComparer.AreStrictlyEquivalent
                    (TestData.NestedObjectCases.sourceJson, TestData.NestedObjectCases.deepStringPropertyDiffers)
            Assert.False(actual)
