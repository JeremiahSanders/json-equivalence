[<AutoOpen>]
module Jds.JsonEquivalence.Tests.Unit.TestData.RootPropertyCases

let sourceJson = """{
  "name": "one",
  "order": -2,
  "threads": ["simple","complex"],
  "unused": null
}
"""
let reordered = """{
  "order": -2,
  "name": "one",
  "threads": ["simple","complex"],
  "unused": null
}
"""
let missingProperty = """{
  "order": -2,
  "threads": ["simple","complex"],
  "unused": null
}
"""
let differingStringValue = """{
  "name": "two",
  "order": -2,
  "threads": ["simple","complex"],
  "unused": null
}
"""
let differingStringCasing = """{
  "name": "ONE",
  "order": -2,
  "threads": ["simple","complex"],
  "unused": null
}
"""
let differingNumericValue = """{
  "name": "one",
  "order": 1,
  "threads": ["simple","complex"],
  "unused": null
}
"""
let arrayMissingElement = """{
  "name": "one",
  "order": -2,
  "threads": ["simple"],
  "unused": null
}
"""
let nullPopulated = """{
  "name": "one",
  "order": -2,
  "threads": ["simple","complex"],
  "unused": false
}
"""
