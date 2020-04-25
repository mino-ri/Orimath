namespace Orimath.BasicTools
open Orimath.Plugins

type BasicToolsProvidor() =
    interface IToolProvidor with
        member __.GetTools(workspace) = upcast ([ DragFoldingTool(workspace) ] : ITool list)
