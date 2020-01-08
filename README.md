# JSON Equivalence

A simple library to support validating JSON object equivalence in tests.

![Build status](https://github.com/JeremiahSanders/json-equivalence/workflows/Build/badge.svg)

## Usage

### Usage in F# Projects

```fsharp
open Jds.JsonEquivalence

let sampleJson = """{"identity":"one"}"""
let otherJson = """{"identity":"One"}"""

let caseInsensitiveOptions = EquivalenceOptions(EquivalenceValueOptions(false), EquivalencePropertyOptions.Default)

EquivalenceComparer.AreStrictlyEquivalent(sampleJson, otherJson)
// false

EquivalenceComparer.AreEquivalent(caseInsensitiveOptions, sampleJson, otherJson)
// true
```

### Usage in C# Projects

```csharp
using Jds.JsonEquivalence;

const string sampleJson = @"{""identity"":""one""}";
const string otherJson = @"{""identity"":""One""}";

// Equivalent to EquivalencePropertyOptions.Default
var propertyOptions = EquivalencePropertyOptions.ofIgnoredPaths(System.Array.Empty<string>());

var caseInsensitiveOptions = new EquivalenceOptions(
  EquivalenceValueOptions.CaseInsensitive,
  propertyOptions
);

EquivalenceComparer.AreStrictlyEquivalent(sampleJson, otherJson);
// false

EquivalenceComparer.AreEquivalent(caseInsensitiveOptions, sampleJson, otherJson);
// true
```
