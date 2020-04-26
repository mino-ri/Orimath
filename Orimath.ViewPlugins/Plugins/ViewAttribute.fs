namespace Orimath.Plugins
open System

[<AttributeUsage(AttributeTargets.Class)>]
type ViewAttribute(pane: ViewPane, viewModelType: Type) =
    inherit Attribute()
    member __.Pane = pane
    member __.ViewModelType = viewModelType
