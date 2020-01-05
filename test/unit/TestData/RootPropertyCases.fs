[<AutoOpen>]
module Jds.JsonEquivalence.Tests.Unit.TestData.RootPropertyCases

let sourceJson = """{
  "active": true,
  "name": "one",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let reordered = """{
  "active": true,
  "order": -2,
  "price": 23.45678901,
  "name": "one",
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let missingProperty = """{
  "active": true,
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let differingStringValue = """{
  "active": true,
  "name": "two",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let differingStringCasing = """{
  "active": true,
  "name": "ONE",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let differingNumericValue = """{
  "active": true,
  "name": "one",
  "order": 1,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let arrayMissingElement = """{
  "active": true,
  "name": "one",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple"],
  "unused": null,
  "visible": false
}
"""
let nullPopulated = """{
  "active": true,
  "name": "one",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": false,
  "visible": false
}
"""
let trueIsFalse = """{
  "active": false,
  "name": "one",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
let falseIsTrue = """{
  "active": true,
  "name": "one",
  "order": -2,
  "price": 23.45678901,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": true
}
"""
let decimalValueTrimmed = """{
  "active": true,
  "name": "one",
  "order": -2,
  "price": 23.456,
  "threads": ["simple","complex"],
  "unused": null,
  "visible": false
}
"""
