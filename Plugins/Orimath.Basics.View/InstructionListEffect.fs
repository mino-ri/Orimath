namespace Orimath.Basics.View
open System.ComponentModel.DataAnnotations
open System.Windows.Media
open Orimath.Basics.Folds
open Orimath.Basics.View.Export
open Orimath.Basics.View.ViewModels
open Orimath.Plugins
open ApplicativeProperty

[<ReferenceEquality; NoComparison>]
type InstructionListSetting =
    {
        [<Display(Name = "{basic/InstructionList.Margin}Margin")>]
        [<Range(1, 5000)>]
        mutable Margin: int

        [<Display(Name = "{basic/InstructionList.PaperSize}Paper size")>]
        [<Range(1, 5000)>]
        mutable PaperSize: int

        [<Display(Name = "{basic/InstructionList.IndexFontSize}Index font size")>]
        [<Range(1, 100)>]
        mutable IndexFontSize: int

        [<Display(Name = "{basic/InstructionList.IndexOffset}Index offset")>]
        [<Range(1, 5000)>]
        mutable IndexOffset: int

        [<Display(Name = "{basic/InstructionList.ColumnCount}Export columns count")>]
        [<Range(1, 20)>]
        mutable ColumnCount: int
    }

type InstructionListEffect
    (workspace: IWorkspace,
     messenger: IMessenger,
     fileManager: IFileManager,
     dispatcher: IDispatcher,
     setting: InstructionListSetting
    ) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.InstructionList}Show instructions..."
        member _.ShortcutKey = ""
        member _.Icon = getIcon "inst_list"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() =
            let drawIndex index offset fontSize (pointConverter: IViewPointConverter) (exporter: IShapeExporter) =
                exporter.AddText(
                    index,
                    pointConverter.ModelToView({ X = offset; Y = 1.0 - offset }),
                    float fontSize,
                    Colors.Black)
            let drawCurrent index indexFontSize indexOffset pointConverter exporter =
                let context = ExportContext(exporter, pointConverter)
                context.DrawPaper(workspace.Paper)
                if index <> 0 then
                    drawIndex (string index) indexOffset indexFontSize pointConverter exporter
            let drawCp _ indexFontSize indexOffset pointConverter exporter =
                let context = ExportContext(exporter, pointConverter)
                context.DrawCreasePattern(workspace.Paper)
                drawIndex "CP" indexOffset indexFontSize pointConverter exporter
            let images =
                workspace.Paper.UndoSnapShots
                |> Seq.map (fun (paper, tag) ->
                    fun index indexFontSize indexOffset pointConverter exporter ->
                            let context = ExportContext(exporter, pointConverter)
                            context.DrawPaper(paper)
                            match tag with
                            | :? FoldOperation as opr -> context.DrawFoldOperation(paper, opr)
                            | _ -> ()
                            if index <> 0 then
                                drawIndex (string index) indexOffset indexFontSize pointConverter exporter)
                |> Seq.append [ drawCp; drawCurrent ]
                |> Seq.rev
                |> Seq.map (fun drawImage -> InstructionItemViewModel(dispatcher, drawImage))
                |> Seq.toList
            let viewModel =
                InstructionListDialogViewModel(
                    messenger,
                    fileManager,
                    dispatcher,
                    images,
                    setting.Margin,
                    setting.PaperSize,
                    setting.IndexFontSize,
                    setting.IndexOffset,
                    setting.ColumnCount)
            dispatcher.UI {
                viewModel.UpdateImage()
                messenger.OpenDialog(viewModel)
            }
