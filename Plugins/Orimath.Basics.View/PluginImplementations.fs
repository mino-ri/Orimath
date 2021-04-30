namespace Orimath.Basics.View
open System.ComponentModel
open Orimath.Plugins
open Orimath.Basics
open Orimath.Basics.Folds
open Orimath.Basics.View.ViewModels
open Orimath.Basics.View.Export

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


type PngExportPluginSetting =
    { Margin: int
      PaperSize: int }


[<DisplayName("{basic/ImageExport.Name}Command: Png export")>]
[<Description("{basic/ImageExport.Desc}Export in png format.")>]
type ImageExportPlugin() =
    inherit ConfigurablePluginBase<PngExportPluginSetting>({ Margin = 128; PaperSize = 512 })
    interface IViewPlugin with
        member this.Execute(args: ViewPluginArgs) =
            let exporter =
                { new IImageExporter with
                    member _.FileTypeName = "{FileType.Png.FileName}png image"
                    member _.Extension = "png"
                    member _.Export(stream, paper) =
                        let margin = this.Setting.Margin
                        let paperSize = this.Setting.PaperSize
                        let imageSize = paperSize + margin * 2
                        async {
                            let drawPaper (exporter: VisualExporter) =
                                let context =
                                    ExportContext(
                                        exporter,
                                        ViewPointConverter(
                                            float paperSize, float -paperSize,
                                            float margin, float (paperSize + margin)))
                                match paper.UndoSnapShots |> Seq.tryHead with
                                | Some(paper, (:? FoldOperation as opr)) ->
                                    context.DrawPaper(paper)
                                    context.DrawFoldOperation(paper, opr)
                                | _ -> context.DrawPaper(paper)
                            VisualExporter.ExportPngToStream(stream, imageSize, imageSize, drawPaper)
                        }
                    member _.ShortcutKey = "ctrl+P"
                    member _.EffectName = "{FileType.Png.EffectName}png export..."
                }
            args.Workspace.AddEffect(ExportEffect(args.FileManager, args.Workspace, exporter))

[<DisplayName("{basic/InstructionList.Name}Command: Show instructions")>]
[<Description("{basic/InstructionList.Desc}Show folding instructions.")>]
type InstructionListPlugin() =
    inherit ConfigurablePluginBase<InstructionListSetting>({ Margin = 64; PaperSize = 256 })
    interface IViewPlugin with
        member this.Execute(args: ViewPluginArgs) =
            args.Workspace.AddEffect(
                InstructionListEffect(args.Workspace, args.Messenger, args.Dispatcher, this.Setting))
            args.Messenger.RegisterView(ViewPane.Dialog, typeof<InstructionListDialogViewModel>,
                viewPath "InstructionListDialogControl")