namespace Orimath.Plugins
open System

[<Sealed; AttributeUsage(AttributeTargets.Class)>]
type OrimathPluginAttribute(name: string, description: string) =
    inherit Attribute()
    member __.Name = name
    member __.Description = description
