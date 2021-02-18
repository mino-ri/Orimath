namespace Orimath.Basics.View
open System.ComponentModel
open Orimath.Plugins
open Orimath.Basics
open Orimath.Basics.View.ViewModels

[<DisplayName("{basic/NewPaper.Name}Command: New paper")>]
[<Description("{basic/NewPaper.Desc}New paper and reset command")>]
type NewPaperPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            let newPaperExecutor = NewPaperExecutor(args.Workspace)
            args.Workspace.AddEffect(newPaperExecutor.NewPaperEffect)
            args.Workspace.AddEffect(newPaperExecutor.ResetEffect)
            args.Messenger.SetEffectParameterViewModel<NewPaperExecutor>(
                fun p -> upcast NewPaperDialogViewModel(args.Messenger, args.Dispatcher, p))
            args.Messenger.RegisterView(ViewPane.Dialog, typeof<NewPaperDialogViewModel>,
                viewPath "NewPaperDialogControl")


[<DisplayName("{basic/PaperView.Name}View: Origami")>]
[<Description("{basic/PaperView.Desc}Main workspace. Please do not disable this plugin.")>]
type BasicViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(WorkspaceViewModel(args.Workspace, args.PointConverter, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Main, typeof<WorkspaceViewModel>, viewPath "PaperControl")


[<DisplayName("{basic/ToolBar.Name}View: Tool bar")>]
[<Description("{basic/ToolBar.Desc}Tool bar for execute commands.")>]
type EffectViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(EffectListViewModel(args.Workspace, args.Messenger))
            args.Messenger.RegisterView(ViewPane.Top, typeof<EffectListViewModel>, viewPath "EffectListControl")


[<DisplayName("{basic/ToolBox.Name}View: Tool box")>]
[<Description("{basic/ToolBox.Desc}Tool box for switch tools.")>]
type ToolViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(ToolListViewModel(args.Messenger, args.Workspace, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Side, typeof<ToolListViewModel>, viewPath "ToolListControl")


[<DisplayName("{basic/MeasurementView.Name}View: Measurement")>]
[<Description("{basic/MeasurementView.Desc}View mathematical infomations of selected points or lines.")>]
type MeasureViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(MeasureViewModel(args.Workspace.Paper, args.PointConverter, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Side, typeof<MeasureViewModel>, viewPath "MeasureControl")


[<DisplayName("{basic/CreasePatternView.Name}View: Crease pattern(CP)")>]
[<Description("{basic/CreasePatternView.Desc}View crease patterns.")>]
type CreasePatternViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(
                CreasePatternViewModel(args.Workspace.Paper, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Side, typeof<CreasePatternViewModel>,
                viewPath "CreasePatternControl")


[<DisplayName("{basic/DiagramView.Name}View: Diagram")>]
[<Description("{basic/DiagramView.Desc}View diagram over the main view.")>]
type FoldingInstructionPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(
                FoldingInstructionViewModel(args.Workspace, args.Dispatcher, args.PointConverter))
            args.Messenger.RegisterView(ViewPane.Main, typeof<FoldingInstructionViewModel>,
                viewPath "FoldingInstructionControl")
