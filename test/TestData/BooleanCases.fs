module Jds.JsonEquivalence.Tests.TestData.BooleanCases

let sourceJson = """{
  "active": true,
  "visible": false
}
"""
let trueIsFalse = """{
  "active": false,
  "visible": false
}
"""
let falseIsTrue = """{
  "active": true,
  "visible": true
}
"""
let reordered = """{
  "visible": false,
  "active": true
}
"""
