namespace Jds.JsonEquivalence.Tests

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

    module TestData =
        let emptyJsonObject = "{}"
        let sourceJson = """{
    "export": false,
    "meta": {
      "owner": "test-user"
    },
    "name": "NameValue",
    "nodes": [
      {
        "children": ["a",4,false],
        "key": "key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      },
      "string value",
      42,
      false,
      null,
      true
    ],
    "order": -1,
    "visible": true
}
"""
        let reorderedArray = """{
    "export": false,
    "meta": {
      "owner": "test-user"
    },
    "name": "NameValue",
    "nodes": [
      {
        "children": ["a",4,false],
        "key": "key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      },
      "string value",
      42,
      null,
      false,
      true
    ],
    "order": -1,
    "visible": true
}
"""
        let reorderedRootProperties = """{
    "order": -1,
    "meta": {
      "owner": "test-user"
    },
    "name": "NameValue",
    "export": false,
    "nodes": [
      {
        "children": ["a",4,false],
        "key": "key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      },
      "string value",
      42,
      false,
      null,
      true
    ],
    "visible": true
}
"""
        let missingRootProperty = """{
    "meta": {
      "owner": "test-user"
    },
    "name": "NameValue",
    "nodes": [
      {
        "children": ["a",4,false],
        "key": "key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      },
      "string value",
      42,
      false,
      null,
      true
    ],
    "order": -1,
    "visible": true
}
"""

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
            [ [| "same string"; strictEquivalence; TestData.sourceJson; TestData.sourceJson; true |]
              [| "source vs empty object"; strictEquivalence; TestData.sourceJson; TestData.emptyJsonObject; false |] ]

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

              [| "booleans - inverted false"; strictEquivalence; TestData.BooleanCases.sourceJson;
                 TestData.BooleanCases.falseIsTrue; false |]

              [| "booleans - inverted true"; strictEquivalence; TestData.BooleanCases.sourceJson;
                 TestData.BooleanCases.trueIsFalse; false |]

              [| "booleans - reordered"; strictEquivalence; TestData.BooleanCases.sourceJson;
                 TestData.BooleanCases.reordered; true |]

              [| "arrays - missing element"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.arrayMissingElement; false |]

              [| "arrays - extra element"; strictEquivalence; TestData.RootPropertyCases.arrayMissingElement;
                 TestData.RootPropertyCases.sourceJson; false |]

              [| "null populated"; strictEquivalence; TestData.RootPropertyCases.sourceJson;
                 TestData.RootPropertyCases.nullPopulated; false |]

              [| "value replaced with null"; strictEquivalence; TestData.RootPropertyCases.nullPopulated;
                 TestData.RootPropertyCases.sourceJson; false |] ]

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
