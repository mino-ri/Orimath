namespace Orimath.Basics.View
open System.Windows.Media.Imaging
open Orimath.Basics.Folds
open Orimath.Basics.View.Export
open Orimath.Basics.View.ViewModels
open Orimath.Plugins
open ApplicativeProperty

type InstructionListSetting =
    { Margin: int
      PaperSize: int }

type InstructionListEffect
    (workspace: IWorkspace,
     messenger: IMessenger,
     dispatcher: IDispatcher,
     setting: InstructionListSetting
    ) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.InstructionList}Show instructions..."
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() =
            let margin = setting.Margin
            let paperSize = setting.PaperSize
            let imageSize = paperSize + margin * 2
            let pointConverter =
                ViewPointConverter(
                    float paperSize, float -paperSize,
                    float margin, float (paperSize + margin))
            let currentImage =
                VisualExporter.ExportToBitmap(imageSize, imageSize, fun exporter ->
                    let context = ExportContext(exporter, pointConverter)
                    context.DrawPaper(workspace.Paper))
            let images =
                workspace.Paper.UndoSnapShots
                |> Seq.map (fun (paper, tag) ->
                    VisualExporter.ExportToBitmap(imageSize, imageSize, fun exporter ->
                        let context = ExportContext(exporter, pointConverter)
                        context.DrawPaper(paper)
                        match tag with
                        | :? FoldOperation as opr -> context.DrawFoldOperation(paper, opr)
                        | _ -> ()) :> BitmapSource)
                |> Seq.append [ currentImage ]
                |> Seq.rev
                |> Seq.toList
            dispatcher.UI {
                messenger.OpenDialog(InstructionListDialogViewModel(messenger, imageSize, images))
            }
