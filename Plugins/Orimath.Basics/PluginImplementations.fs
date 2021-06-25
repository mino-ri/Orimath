namespace Orimath.Basics
open System.ComponentModel
open Orimath.Plugins

[<DisplayName("{basic/BasicCommands.Name}Command: Basics")>]
[<Description("{basic/BasicCommands.Desc}Basic commands e.g. undo and redo")>]
type BasicPlugin() =
    interface IPlugin with
        member _.Execute(args) =
            args.Workspace.AddEffect(OpenEffect(args.FileManager))
            args.Workspace.AddEffect(SaveAsEffect(args.FileManager))
            args.Workspace.AddEffect(SaveEffect(args.FileManager))
            args.Workspace.AddEffect(UndoEffect(args.Workspace))
            args.Workspace.AddEffect(RedoEffect(args.Workspace))
            args.Workspace.AddEffect(RotateLeftEffect(args.Workspace))
            args.Workspace.AddEffect(RotateRightEffect(args.Workspace))
            args.Workspace.AddEffect(TurnVerticallyEffect(args.Workspace))
            args.Workspace.AddEffect(TurnHorizontallyEffect(args.Workspace))
            args.Workspace.AddEffect(OpenAllEffect(args.Workspace))


[<DisplayName("{basic/Folding.Name}Tool: Folding")>]
[<Description("{basic/Folding.Desc}Fold by dragging")>]
type FoldsPlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(Folds.DragFoldTool(args.Workspace))


[<DisplayName("{basic/Draft.Name}Tool: Draft line")>]
[<Description("{basic/Draft.Desc}Make draft lines by dragging")>]
type DraftPlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(Folds.DragDraftLineTool(args.Workspace))


[<DisplayName("{basic/Measurement.Name}Tool: Measurement")>]
[<Description("{basic/Measurement.Desc}Measure angles and distances by dragging")>]
type MeasurePlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(MeasureTool(args.Workspace))
