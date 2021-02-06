[<AutoOpen>]
module Orimath.Internal

let isNotNull x = not (isNull x)

let inline ( ?|| ) a b = if isNotNull a then a else b

let inline ( ?|> ) a mapping = if isNotNull a then mapping a else null
