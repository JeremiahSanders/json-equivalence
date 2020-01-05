namespace Jds.JsonEquivalence.Tests.Unit

module JsonComparisonTests =
    open System
    open System.Collections.Generic
    open Xunit
    open Xunit.Abstractions
    open Jds.JsonEquivalence
    open Jds.JsonEquivalence.JsonComparison

    let toObjectArraySingleton value : Object array = value :> Object |> Array.singleton

    let toObjectArray<'a> (valueList : 'a list) : Object array =
        valueList
        |> List.map (fun value -> (value :> Object))
        |> List.toArray

    let strictEquivalence = EquivalenceOptions.Strict
    let caseInsensitive : EquivalenceOptions = EquivalenceOptions(EquivalenceValueOptions(false))

    type SerializableRecordFromGameMessage(output : ITestOutputHelper) =

        static member ArrayTestCases : obj [] list =
            [ [| "reordered array"; strictEquivalence; TestData.ArrayCases.numericArray;
                 TestData.ArrayCases.numericArrayReordered; false |]

              [| "left empty, right not"; strictEquivalence; TestData.ArrayCases.emptyArray;
                 TestData.ArrayCases.numericArray; false |]

              [| "right empty, left not"; strictEquivalence; TestData.ArrayCases.numericArray;
                 TestData.ArrayCases.emptyArray; false |]

              [| "spaces between elements in numeric array"; strictEquivalence; TestData.ArrayCases.numericArray;
                 TestData.ArrayCases.numericArrayWithSpaces; true |]

              [| "spaces between elements in string array"; strictEquivalence; TestData.ArrayCases.stringArray;
                 TestData.ArrayCases.stringArrayWithSpaces; true |]

              [| "missing element"; strictEquivalence; TestData.ArrayCases.numericArray;
                 TestData.ArrayCases.numericArrayMissingElement; false |]

              [| "extra element"; strictEquivalence; TestData.ArrayCases.numericArrayMissingElement;
                 TestData.ArrayCases.numericArray; false |] ]

        static member NestedObjectTestCases : obj [] list =
            [ [| "child property reordered"; strictEquivalence; TestData.NestedObjectCases.sourceJson;
                 TestData.NestedObjectCases.childPropertyReordered; true |]

              [| "deep string property differs"; strictEquivalence; TestData.NestedObjectCases.sourceJson;
                 TestData.NestedObjectCases.deepStringPropertyDiffers; false |] ]

        static member SimpleTestCases : obj [] list =
            let emptyJsonMinified = "{}"
            let emptyJsonVerbose = """{

}
"""
            [ [| "same string"; strictEquivalence; emptyJsonMinified; emptyJsonMinified; true |]
              [| "only whitespace difference"; strictEquivalence; emptyJsonMinified; emptyJsonVerbose; true |]

              [| "source vs empty object"; strictEquivalence; TestData.RootPropertyCases.sourceJson; emptyJsonMinified;
                 false |] ]

        static member RootPropertyTestCases : obj [] list =
            [ [| "reordered"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.reordered; true |]

              [| "missing root property"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.missingProperty; false |]

              [| "extra root property"; strictEquivalence; TestData.RootPropertyCases.missingProperty;
                 TestData.RootPropertyCases.sourceJson; false |]

              [| "differing numeric value"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.differingNumericValue; false |]

              [| "differing string value"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.differingStringValue; false |]

              [| "differing string casing - strict"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.differingStringCasing; false |]

              [| "differing string casing - value.caseInsensitive"; caseInsensitive;
                 TestData.RootPropertyCases.sourceJson; TestData.RootPropertyCases.differingStringCasing; true |]

              [| "booleans - inverted false"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.falseIsTrue; false |]

              [| "booleans - inverted true"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.trueIsFalse; false |]

              [| "arrays - missing element"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.arrayMissingElement; false |]

              [| "arrays - extra element"; strictEquivalence; TestData.RootPropertyCases.arrayMissingElement;
                 TestData.RootPropertyCases.sourceJson; false |]

              [| "null populated"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.nullPopulated; false |]

              [| "value replaced with null"; strictEquivalence; TestData.RootPropertyCases.nullPopulated;
                 TestData.RootPropertyCases.sourceJson; false |]

              [| "decimal value trimmed"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.decimalValueTrimmed; false |] ]

        [<Theory>]
        [<MemberData("ArrayTestCases")>]
        member this.``Correctly identifies array equivalence`` (testCaseTitle : string) (options : EquivalenceOptions)
               (jsonLeft : string) (jsonRight : string) (shouldMatch : bool) =
            let actual = JsonComparison.compare options jsonLeft jsonRight
            output.WriteLine(sprintf "%s: %b %b" testCaseTitle shouldMatch actual)
            Assert.Equal(shouldMatch, actual)

        [<Theory>]
        [<MemberData("NestedObjectTestCases")>]
        member this.``Correctly identifies nested object equivalence`` (testCaseTitle : string)
               (options : EquivalenceOptions) (jsonLeft : string) (jsonRight : string) (shouldMatch : bool) =
            let actual = JsonComparison.compare options jsonLeft jsonRight
            output.WriteLine(sprintf "%s: %b %b" testCaseTitle shouldMatch actual)
            Assert.Equal(shouldMatch, actual)

        [<Theory>]
        [<MemberData("RootPropertyTestCases")>]
        member this.``Correctly identifies root property equivalence`` (testCaseTitle : string)
               (options : EquivalenceOptions) (jsonLeft : string) (jsonRight : string) (shouldMatch : bool) =
            let actual = JsonComparison.compare options jsonLeft jsonRight
            output.WriteLine(sprintf "%s: %b %b" testCaseTitle shouldMatch actual)
            Assert.Equal(shouldMatch, actual)

        [<Theory>]
        [<MemberData("SimpleTestCases")>]
        member this.``Correctly identifies simple test case equivalence`` (testCaseTitle : string)
               (options : EquivalenceOptions) (jsonLeft : string) (jsonRight : string) (shouldMatch : bool) =
            let actual = JsonComparison.compare options jsonLeft jsonRight
            output.WriteLine(sprintf "%s: %b %b" testCaseTitle shouldMatch actual)
            Assert.Equal(shouldMatch, actual)
