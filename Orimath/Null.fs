[<RequireQualifiedAccess>]
module internal Orimath.Null

let iter action nullable = if isNull nullable then () else action nullable

let bind mapping nullable = if isNull nullable then null else mapping nullable

let mapv mapping nullable = if isNull nullable then None else Some(mapping nullable)

let filter pred nullable = if isNull nullable || pred nullable then null else nullable

let defaultValue value nullable = if isNull nullable then value else nullable

let defaultWith value nullable = if isNull nullable then value() else nullable
