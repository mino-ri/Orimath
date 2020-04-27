namespace Orimath.Plugins
open System

type ViewPane =
    | Main = 0
    | Top = 1
    | Side = 2
    | Dialog = 3

[<AttributeUsage(AttributeTargets.Class)>]
type ViewAttribute(pane: ViewPane, viewModelType: Type) =
    inherit Attribute()
    member __.Pane = pane
    member __.ViewModelType = viewModelType
