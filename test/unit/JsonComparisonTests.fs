namespace Jds.JsonEquivalence.Tests.Unit

module JsonComparisonTests =
    open System
    open Xunit
    open Xunit.Abstractions
    open Jds.JsonEquivalence

    let toObjectArraySingleton value : Object array = value :> Object |> Array.singleton

    let toObjectArray<'a> (valueList : 'a list) : Object array =
        valueList
        |> List.map (fun value -> (value :> Object))
        |> List.toArray

    let strictEquivalence = EquivalenceOptions.Strict
    let caseInsensitive : EquivalenceOptions =
        EquivalenceOptions(EquivalenceValueOptions(false), EquivalencePropertyOptions.Default)
    let strictIgnoreId =
        EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "id" ]))
    let caseInsensitiveIgnoreId : EquivalenceOptions =
        EquivalenceOptions(EquivalenceValueOptions(false), EquivalencePropertyOptions.ofIgnoredPaths ([ "id" ]))
    let strictIgnoreOwnerName =
        EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "owner.name" ]))
    let caseInsensitiveOwnerName : EquivalenceOptions =
        EquivalenceOptions(EquivalenceValueOptions(false), EquivalencePropertyOptions.ofIgnoredPaths ([ "owner.name" ]))
    let strictIgnoreTiming =
        EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "timing" ]))
    let strictIgnoreTiming0 =
        EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "timing[0]" ]))
    let strictIgnoreTiming1 =
        EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "timing[1]" ]))
    let strictIgnoreTiming1Key =
        EquivalenceOptions
            (EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "timing[1].key" ]))
    let strictIgnoreTiming1Value =
        EquivalenceOptions
            (EquivalenceValueOptions.Strict, EquivalencePropertyOptions.ofIgnoredPaths ([ "timing[1].value" ]))

    type CompareTests(output : ITestOutputHelper) =

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

        static member IgnoredPropertyTestCases : obj [] list =
            [ [| "different id - strict"; strictEquivalence; TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentId; false |]

              [| "different id - strictIgnoreId"; strictIgnoreId; TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentId; true |]

              [| "different owner name - strict"; strictEquivalence; TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentOwnerName; false |]

              [| "different owner name - caseInsensitive"; caseInsensitive; TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentOwnerName; false |]

              [| "different owner name - caseInsensitiveOwnerName"; caseInsensitiveOwnerName;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentOwnerName; true |]

              [| "different owner name - strictIgnoreOwnerName"; strictIgnoreOwnerName;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentOwnerName; true |]

              [| "different owner name casing - strict"; strictEquivalence; TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentOwnerNameCasing; false |]

              [| "different owner name casing - caseInsensitive"; caseInsensitive;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentOwnerNameCasing; true |]

              [| "different owner name casing - caseInsensitiveOwnerName"; caseInsensitiveOwnerName;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentOwnerNameCasing; true |]

              [| "different owner name casing - strictIgnoreOwnerName"; strictIgnoreOwnerName;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentOwnerNameCasing; true |]

              [| "different array element string casing - strict"; strictEquivalence;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentArrayStringCasing;
                 false |]

              [| "different array element string casing - caseInsensitive"; caseInsensitive;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentArrayStringCasing;
                 true |]

              [| "different array element string casing - strictIgnoreTiming"; strictIgnoreTiming;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentArrayStringCasing;
                 true |]

              [| "different array element string casing - strictIgnoreTiming0"; strictIgnoreTiming0;
                 TestData.IgnoredPropertyCases.sourceJson; TestData.IgnoredPropertyCases.differentArrayStringCasing;
                 true |]

              [| "different array element nested property value - strict"; strictEquivalence;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentArrayElementNestedPropertyValue; false |]

              [| "different array element nested property value - caseInsensitive"; caseInsensitive;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentArrayElementNestedPropertyValue; false |]

              [| "different array element nested property value - strictIgnoreTiming"; strictIgnoreTiming;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentArrayElementNestedPropertyValue; true |]

              [| "different array element nested property value - strictIgnoreTiming1"; strictIgnoreTiming1;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentArrayElementNestedPropertyValue; true |]

              [| "different array element nested property value - strictIgnoreTiming1Key"; strictIgnoreTiming1Key;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentArrayElementNestedPropertyValue; true |]

              [| "different array element nested property value - strictIgnoreTiming1Value"; strictIgnoreTiming1Value;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.differentArrayElementNestedPropertyValue; false |]

              [| "array element nested property missing - strict"; strictEquivalence;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.arrayElementNestedPropertyMissing; false |]

              [| "array element nested property missing - caseInsensitive"; caseInsensitive;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.arrayElementNestedPropertyMissing; false |]

              [| "array element nested property missing - strictIgnoreTiming"; strictIgnoreTiming;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.arrayElementNestedPropertyMissing; true |]

              [| "array element nested property missing - strictIgnoreTiming1"; strictIgnoreTiming1;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.arrayElementNestedPropertyMissing; true |]

              [| "array element nested property missing - strictIgnoreTiming1Key"; strictIgnoreTiming1Key;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.arrayElementNestedPropertyMissing; false |]

              [| "array element nested property missing - strictIgnoreTiming1Value"; strictIgnoreTiming1Value;
                 TestData.IgnoredPropertyCases.sourceJson;
                 TestData.IgnoredPropertyCases.arrayElementNestedPropertyMissing; true |] ]

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
        [<MemberData("IgnoredPropertyTestCases")>]
        member this.``Correctly identifies ignored property object equivalence`` (testCaseTitle : string)
               (options : EquivalenceOptions) (jsonLeft : string) (jsonRight : string) (shouldMatch : bool) =
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
