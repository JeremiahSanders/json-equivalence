namespace Jds.JsonEquivalence.Tests.Unit.TestData

module IgnoredPropertyCases =
    let sourceJson = """{
  "id":42,
  "owner":{
    "id":128,
    "name":"Owner"
  },
  "timing":[
    "simple",
    {
      "key":"Width",
      "value":"thin"
    }
  ]
}
"""
    let differentId = """{
  "id":1,
  "owner":{
    "id":128,
    "name":"Owner"
  },
  "timing":[
    "simple",
    {
      "key":"Width",
      "value":"thin"
    }
  ]
}
"""
    let differentOwnerName = """{
  "id":42,
  "owner":{
    "id":128,
    "name":"Different Name"
  },
  "timing":[
    "simple",
    {
      "key":"Width",
      "value":"thin"
    }
  ]
}
"""
    let differentOwnerNameCasing = """{
  "id":42,
  "owner":{
    "id":128,
    "name":"owner"
  },
  "timing":[
    "simple",
    {
      "key":"Width",
      "value":"thin"
    }
  ]
}
"""
    let differentOwnerId = """{
  "id":42,
  "owner":{
    "id":17,
    "name":"Owner"
  },
  "timing":[
    "simple",
    {
      "key":"Width",
      "value":"thin"
    }
  ]
}
"""
    let differentArrayStringCasing = """{
  "id":42,
  "owner":{
    "id":128,
    "name":"Owner"
  },
  "timing":[
    "Simple",
    {
      "key":"Width",
      "value":"thin"
    }
  ]
}
"""
    let differentArrayElementNestedPropertyValue = """{
  "id":42,
  "owner":{
    "id":128,
    "name":"Owner"
  },
  "timing":[
    "simple",
    {
      "key":"Border",
      "value":"thin"
    }
  ]
}
"""
    let arrayElementNestedPropertyMissing = """{
  "id":42,
  "owner":{
    "id":128,
    "name":"Owner"
  },
  "timing":[
    "simple",
    {
      "key":"Width"
    }
  ]
}
"""
