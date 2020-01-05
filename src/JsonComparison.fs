namespace Jds.JsonEquivalence

open System
open System.Text.Json

type EquivalenceValueOptions(caseSensitive : bool) =
    member this.CaseSensitive = caseSensitive
    static member Strict = EquivalenceValueOptions(true)

type EquivalenceOptions(valueOptions : EquivalenceValueOptions) =
    member this.Values = valueOptions
    static member Strict = EquivalenceOptions(EquivalenceValueOptions.Strict)

module JsonComparison =
    let explicitComparer expected actual = expected = actual

    let private compareStrings (options : EquivalenceOptions) (expected : string) (actual : string) =
        if (options.Values.CaseSensitive) then StringComparison.InvariantCulture
        else StringComparison.InvariantCultureIgnoreCase
        |> (fun stringComparison -> expected.Equals(actual, stringComparison))

    let private areNumbersEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        expected.GetDecimal() = actual.GetDecimal() || expected.GetInt64() = actual.GetInt64()
    let private areStringsEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        compareStrings options (expected.GetString()) (actual.GetString())

    let rec private arePropertiesEquivalent (options : EquivalenceOptions) (expected : JsonProperty)
            (actual : JsonProperty) =
        (compareStrings options expected.Name actual.Name)
        && (areElementsEquivalent options expected.Value actual.Value)

    and private areObjectsEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        let hasEquivalentProperty (expectedProperty : JsonProperty) (actualElement : JsonElement) =
            let hasProperty, propertyValue = actualElement.TryGetProperty(expectedProperty.Name)
            hasProperty && (areElementsEquivalent options expectedProperty.Value propertyValue)

        let compareObjects (expected : JsonElement) (actual : JsonElement) =
            expected.EnumerateObject()
            |> Seq.tryFind (fun property -> not (hasEquivalentProperty property actual))
            |> Option.map (fun _ -> false)
            |> Option.defaultValue true

        (// Check both directions to ensure no extra properties exist
         compareObjects expected actual) && (compareObjects actual expected)

    and private areArraysEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        let rec compareEnumeratedArrays (expectedArray : JsonElement.ArrayEnumerator)
                (actualArray : JsonElement.ArrayEnumerator) =
            let mutable expected = expectedArray
            let mutable actual = actualArray
            let currentEquivalent = areElementsEquivalent options expectedArray.Current actualArray.Current
            let expectedCanMove = expected.MoveNext()
            let actualCanMove = actual.MoveNext()
            (currentEquivalent) && (expectedCanMove = actualCanMove)
            && ((not expectedCanMove) || (compareEnumeratedArrays expected actual))
        compareEnumeratedArrays (expected.EnumerateArray()) (actual.EnumerateArray())

    and private areValuesEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        match expected.ValueKind with
        | JsonValueKind.Array -> areArraysEquivalent options expected actual
        | JsonValueKind.False -> actual.ValueKind = JsonValueKind.False
        | JsonValueKind.Null -> actual.ValueKind = JsonValueKind.Null
        | JsonValueKind.Number -> areNumbersEquivalent options expected actual
        | JsonValueKind.Object -> areObjectsEquivalent options expected actual
        | JsonValueKind.String -> areStringsEquivalent options expected actual
        | JsonValueKind.True -> actual.ValueKind = JsonValueKind.True
        | JsonValueKind.Undefined -> actual.ValueKind = JsonValueKind.Undefined
        | _ -> false

    and areElementsEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        areValuesEquivalent options expected actual

    let mutable private parsingOptions = JsonDocumentOptions()

    parsingOptions.AllowTrailingCommas <- true
    parsingOptions.CommentHandling <- JsonCommentHandling.Skip

    let jsonComparer (options : EquivalenceOptions) (expected : string) (actual : string) =
        try
            let expectedDocument : JsonDocument = System.Text.Json.JsonDocument.Parse(expected, parsingOptions)
            let actualDocument : JsonDocument = System.Text.Json.JsonDocument.Parse(actual, parsingOptions)
            areElementsEquivalent options expectedDocument.RootElement actualDocument.RootElement
        with
        | :? JsonException as jsonException -> false
        | :? ArgumentException as argumentException -> false

    let compare (options : EquivalenceOptions) expected actual =
        [ explicitComparer
          (jsonComparer options) ]
        |> List.tryPick (fun comparer ->
               if (comparer expected actual) then Some comparer
               else None)
        |> Option.isSome
