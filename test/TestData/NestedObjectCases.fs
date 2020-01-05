module Jds.JsonEquivalence.Tests.TestData.NestedObjectCases

let sourceJson = """{
    "meta": {
      "owner": "test-user",
      "version": 3.14159
    },
    "name": "NameValue",
    "nodes": [
      "string value",
      {
        "children": ["a",4,false],
        "key": "key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      }
    ]
}
"""
let childPropertyReordered = """{
    "meta": {
      "version": 3.14159,
      "owner": "test-user"
    },
    "name": "NameValue",
    "nodes": [
      "string value",
      {
        "children": ["a",4,false],
        "key": "key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      }
    ]
}
"""
let deepStringPropertyDiffers = """{
    "meta": {
      "owner": "test-user",
      "version": 3.14159
    },
    "name": "NameValue",
    "nodes": [
      "string value",
      {
        "children": ["a",4,false],
        "key": "not-the-right-key-name",
        "meta": {
          "owner": "root"
        },
        "value": 5
      }
    ]
}
"""
