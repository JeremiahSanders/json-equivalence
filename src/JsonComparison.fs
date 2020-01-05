namespace Jds.JsonEquivalence

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Text.Json

/// Options controlling the equivalence comparison of JSON values.
type EquivalenceValueOptions(caseSensitive : bool) =

    /// <summary>Are string values comparisons case sensitive.</summary>
    /// <remarks>Does not affect property name casing.</remarks>
    member this.CaseSensitive = caseSensitive

    new() = EquivalenceValueOptions(true)

    /// Value options with string value case sensitivity.
    static member Strict = EquivalenceValueOptions(true)

    /// Value options with string value case insensitivity.
    static member CaseInsensitive = EquivalenceValueOptions(false)

    /// <summary>Default value options. Equal to <see cref="EquivalenceValueOptions.Strict" />.</summary>
    static member Default = EquivalenceValueOptions.Strict

/// Options controlling the equivalence comparison of JSON object properties.
type EquivalencePropertyOptions(ignoredPropertyPaths : IEnumerable<string>) =

    member internal this.IgnoredPropertyPaths : string list =
        match ignoredPropertyPaths with
        | null -> List.empty
        | nonNullEnumerable ->
            nonNullEnumerable
            |> Seq.filter (String.IsNullOrWhiteSpace >> not)
            |> Seq.distinct
            |> Seq.toList

    /// <summary>JSON object property paths to ignore.</summary>
    /// <remarks>Property paths should following a pattern like: <c>propertyName.child[2].subProperty</c>.</remarks>
    member this.IgnoredPaths : IReadOnlyCollection<string> =
        List(this.IgnoredPropertyPaths)
        |> (fun ignoredPaths -> ReadOnlyCollection(ignoredPaths) :> IReadOnlyCollection<string>)

    /// Determines if a path is ignored.
    member this.IsPropertyPathIgnored path = this.IgnoredPropertyPaths |> List.contains path

    /// Creates a new <see cref="EquivalencePropertyOptions" /> from the provided ignored property paths.
    static member ofIgnoredPaths ignoredPropertyPaths = EquivalencePropertyOptions ignoredPropertyPaths

    /// Property options with no ignored property paths.
    static member NoIgnoredPaths = EquivalencePropertyOptions(Seq.empty)

    /// <summary>Default property options. Equal to <see cref="EquivalencePropertyOptions.NoIgnoredPaths" />.</summary>
    static member Default = EquivalencePropertyOptions.NoIgnoredPaths

/// Options controlling the equivalence comparison of JSON documents.
type EquivalenceOptions(valueOptions, propertyOptions) =
    new(valueOptions) = EquivalenceOptions(valueOptions, EquivalencePropertyOptions.Default)
    new(propertyOptions) = EquivalenceOptions(EquivalenceValueOptions.Default, propertyOptions)
    new() = EquivalenceOptions(EquivalenceValueOptions.Default, EquivalencePropertyOptions.Default)

    /// Value equivalence comparison options.
    member this.Values = valueOptions

    /// Object property equivalence comparison options.
    member this.Properties = propertyOptions

    /// <summary>Equivalence options using <see cref="EquivalenceValueOptions.Strict" /> and <see cref="EquivalencePropertyOptions.Default" />.</summary>
    static member Strict = EquivalenceOptions(EquivalenceValueOptions.Strict, EquivalencePropertyOptions.Default)

    /// <summary>Equivalence options using <see cref="EquivalenceValueOptions.Default" /> and <see cref="EquivalencePropertyOptions.Default" />.</summary>
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

    /// Compares two JSON documents for equivalence.
    let compare (options : EquivalenceOptions) expected actual =
        [ equalityComparer
          (jsonComparer options) ]
        |> List.tryPick (fun comparer ->
               if (comparer expected actual) then Some comparer
               else None)
        |> Option.isSome
