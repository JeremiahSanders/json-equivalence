namespace Jds.JsonEquivalence

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Text.Json

type EquivalenceValueOptions(caseSensitive : bool) =
    member this.CaseSensitive = caseSensitive
    new() = EquivalenceValueOptions(true)
    static member Strict = EquivalenceValueOptions(true)
    static member CaseInsensitive = EquivalenceValueOptions(false)
    static member Default = EquivalenceValueOptions.Strict

type EquivalencePropertyOptions(ignoredPropertyPaths : IEnumerable<string>) =

    member internal this.IgnoredPropertyPaths : string list =
        match ignoredPropertyPaths with
        | null -> List.empty
        | nonNullEnumerable ->
            nonNullEnumerable
            |> Seq.filter (String.IsNullOrWhiteSpace >> not)
            |> Seq.distinct
            |> Seq.toList

    member this.IgnoredPaths : IReadOnlyCollection<string> =
        List(this.IgnoredPropertyPaths)
        |> (fun ignoredPaths -> ReadOnlyCollection(ignoredPaths) :> IReadOnlyCollection<string>)
    member this.IsPropertyPathIgnored path = this.IgnoredPropertyPaths |> List.contains path
    static member ofIgnoredPaths ignoredPropertyPaths = EquivalencePropertyOptions ignoredPropertyPaths
    static member Default = EquivalencePropertyOptions(Seq.empty)

type EquivalenceOptions(valueOptions, propertyOptions) =
    new(valueOptions) = EquivalenceOptions(valueOptions, EquivalencePropertyOptions.Default)
    new(propertyOptions) = EquivalenceOptions(EquivalenceValueOptions.Default, propertyOptions)
    new() = EquivalenceOptions(EquivalenceValueOptions.Default, EquivalencePropertyOptions.Default)
    member this.Values = valueOptions
    member this.Properties = propertyOptions
    static member Strict = EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.Default)
    static member Default = EquivalenceOptions(EquivalenceValueOptions.Default, EquivalencePropertyOptions.Default)

module JsonComparison =
    let equalityComparer expected actual = expected = actual

    let private compareStrings (options : EquivalenceOptions) (expected : string) (actual : string) =
        if (options.Values.CaseSensitive) then StringComparison.InvariantCulture
        else StringComparison.InvariantCultureIgnoreCase
        |> (fun stringComparison -> expected.Equals(actual, stringComparison))

    let private areNumbersEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        let areInt64Equivalent() =
            try
                expected.GetInt64() = actual.GetInt64()
            with :? FormatException as formatException -> false

        let areDecimalEquivalent() =
            try
                expected.GetDecimal() = actual.GetDecimal()
            with :? FormatException as formatException -> false

        areDecimalEquivalent() || areInt64Equivalent()

    let private areStringsEquivalent (options : EquivalenceOptions) (expected : JsonElement) (actual : JsonElement) =
        compareStrings options (expected.GetString()) (actual.GetString())

    let rec private areObjectsEquivalent (path : string) (options : EquivalenceOptions) (expected : JsonElement)
            (actual : JsonElement) =
        let createChildPath propertyName =
            match path with
            | "" -> propertyName
            | _ -> sprintf "%s.%s" path propertyName

        let hasEquivalentProperty (expectedProperty : JsonProperty) (actualElement : JsonElement) =
            match options.Properties.IsPropertyPathIgnored(createChildPath expectedProperty.Name) with
            | true -> true
            | false ->
                let hasProperty, propertyValue = actualElement.TryGetProperty(expectedProperty.Name)
                hasProperty
                && (areElementsEquivalent (createChildPath expectedProperty.Name) options expectedProperty.Value
                        propertyValue)

        let compareObjects (expected : JsonElement) (actual : JsonElement) =
            expected.EnumerateObject()
            |> Seq.tryFind (fun property -> not (hasEquivalentProperty property actual))
            |> Option.map (fun _ -> false)
            |> Option.defaultValue true

        (// Check both directions to ensure no extra properties exist
         compareObjects expected actual) && (compareObjects actual expected)

    and private areArraysEquivalent (path : string) (options : EquivalenceOptions) (expected : JsonElement)
        (actual : JsonElement) =
        let rec compareEnumeratedArrays (enumeratorIndex : int) (expectedArray : JsonElement.ArrayEnumerator)
                (actualArray : JsonElement.ArrayEnumerator) =
            let mutable expected = expectedArray
            let mutable actual = actualArray
            let indexPath = sprintf "%s[%d]" path enumeratorIndex
            let currentEquivalent = areElementsEquivalent indexPath options expectedArray.Current actualArray.Current
            let expectedCanMove = expected.MoveNext()
            let actualCanMove = actual.MoveNext()
            (currentEquivalent) && (expectedCanMove = actualCanMove)
            && ((not expectedCanMove) || (compareEnumeratedArrays (enumeratorIndex + 1) expected actual))
        compareEnumeratedArrays (-1) (expected.EnumerateArray()) (actual.EnumerateArray())

    and private areValuesEquivalent (path : string) (options : EquivalenceOptions) (expected : JsonElement)
        (actual : JsonElement) =
        match (options.Properties.IsPropertyPathIgnored(path)) with
        | true -> true
        | false ->
            match expected.ValueKind with
            | JsonValueKind.Array -> areArraysEquivalent path options expected actual
            | JsonValueKind.False -> actual.ValueKind = JsonValueKind.False
            | JsonValueKind.Null -> actual.ValueKind = JsonValueKind.Null
            | JsonValueKind.Number -> areNumbersEquivalent options expected actual
            | JsonValueKind.Object -> areObjectsEquivalent path options expected actual
            | JsonValueKind.String -> areStringsEquivalent options expected actual
            | JsonValueKind.True -> actual.ValueKind = JsonValueKind.True
            | JsonValueKind.Undefined -> actual.ValueKind = JsonValueKind.Undefined
            | _ -> false

    and private areElementsEquivalent (path : string) (options : EquivalenceOptions) (expected : JsonElement)
        (actual : JsonElement) = areValuesEquivalent path options expected actual

    let mutable private parsingOptions = JsonDocumentOptions()

    parsingOptions.AllowTrailingCommas <- true
    parsingOptions.CommentHandling <- JsonCommentHandling.Skip

    let jsonComparer (options : EquivalenceOptions) (expected : string) (actual : string) =
        try
            let expectedDocument : JsonDocument = System.Text.Json.JsonDocument.Parse(expected, parsingOptions)
            let actualDocument : JsonDocument = System.Text.Json.JsonDocument.Parse(actual, parsingOptions)
            areElementsEquivalent "" options expectedDocument.RootElement actualDocument.RootElement
        with
        | :? JsonException as jsonException -> false
        | :? ArgumentException as argumentException -> false

    let compare (options : EquivalenceOptions) expected actual =
        [ equalityComparer
          (jsonComparer options) ]
        |> List.tryPick (fun comparer ->
               if (comparer expected actual) then Some comparer
               else None)
        |> Option.isSome
