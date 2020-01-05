[<AutoOpen>]
module Jds.JsonEquivalence.Tests.Unit.TestData.ArrayCases

let emptyArray = "[]"
let numericArray = "[1,2,3]"
let numericArrayMissingElement = "[1,2]"
let numericArrayWithSpaces = "[1, 2, 3   ]"
let numericArrayReordered = "[2,1,3]"
let stringArray = """["here","I","am"]"""
let stringArrayWithSpaces = """[  "here", "I","am"]"""
