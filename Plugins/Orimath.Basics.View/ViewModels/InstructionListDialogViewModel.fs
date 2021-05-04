namespace Orimath.Basics.View.ViewModels
open System.Windows.Media.Imaging
open Orimath.Controls
open Orimath.Plugins
open Orimath.Basics.View.Export
open ApplicativeProperty


type InstructionItemViewModel
    (dispatcher: IDispatcher,
     draw: int -> int -> float -> IViewPointConverter -> IShapeExporter -> unit) =
    inherit NotifyPropertyChanged()
    member val IsExportTarget = ValueProp(true, dispatcher.SyncContext)
    member val Image = ValueProp<BitmapSource>(null, dispatcher.SyncContext)

    member this.UpdateImage(imageSize, index, indexFontSize, indexOffset, pointConverter) =
        dispatcher.Background {
            this.Image.Value <-
                VisualExporter.ExportToBitmap(imageSize, imageSize,
                    draw index indexFontSize indexOffset pointConverter)
        }
    member _.DrawTo(index, indexFontSize, indexOffset, pointConverter, exporter) =
        draw index indexFontSize indexOffset pointConverter exporter


type InstructionListDialogViewModel
    (messenger: IMessenger,
     fileManager: IFileManager,
     dispatcher: IDispatcher,
     images: InstructionItemViewModel list,
     margin: int,
     paperSize: int,
     indexFontSize: int,
     indexOffset: int,
     columnCount: int) as this =
    inherit NotifyPropertyChanged()
    let margin = ValueProp(margin, dispatcher.SyncContext)
    let paperSize = ValueProp(paperSize, dispatcher.SyncContext)
    let indexFontSize = ValueProp(indexFontSize, dispatcher.SyncContext)
    let indexOffset = ValueProp(indexOffset, dispatcher.SyncContext)
    let columnCount = ValueProp(columnCount, dispatcher.SyncContext)

    member val UpdateImageCommand = Prop.commands this.UpdateImage Prop.ctrue dispatcher.SyncContext
    member val ExportSinglePngCommand = Prop.commands this.ExportSinglePng Prop.ctrue dispatcher.SyncContext
    member _.Header = messenger.LocalizeWord("{basic/Effect.InstructionList}Show instructions...")
    member _.SettingText = messenger.LocalizeWord("{basic/InstructionList.Settings}Settings")
    member _.MarginText = messenger.LocalizeWord("{basic/InstructionList.Margin}Margin")
    member _.PaperSizeText = messenger.LocalizeWord("{basic/InstructionList.PaperSize}Paper size")
    member _.IndexFontSizeText = messenger.LocalizeWord("{basic/InstructionList.IndexFontSize}Index font size")
    member _.IndexOffsetText = messenger.LocalizeWord("{basic/InstructionList.IndexOffset}Index offset")
    member _.ColumnCountText = messenger.LocalizeWord("{basic/InstructionList.ColumnCount}Export columns count")
    member _.ExportSinglePngText =
        messenger.LocalizeWord("{basic/InstructionList.ExportSinglePng}Export in single png file")
    member _.ExportSingleSvgText =
        messenger.LocalizeWord("{basic/InstructionList.ExportSingleSvg}Export in single svg file")
    member _.RegenerateText = messenger.LocalizeWord("{basic/InstructionList.Regenerate}Regenerate")
    member _.OkText = messenger.LocalizeWord("{Ok}OK")
    member _.Images = images
    member _.CloseCommand = messenger.CloseDialogCommand
    member _.Margin = margin
    member _.PaperSize = paperSize
    member _.IndexFontSize = indexFontSize
    member _.IndexOffset = indexOffset
    member _.ColumnCount = columnCount

    member _.UpdateImage(_: obj) =
        let imageSize = paperSize.Value + margin.Value * 2
        let pointConverter =
            ViewPointConverter(
                float paperSize.Value, float -paperSize.Value,
                float margin.Value, float (paperSize.Value + margin.Value))
        let modelIndexOffset =
            float (indexOffset.Value - margin.Value) / float paperSize.Value
        let rec updateImage (images: InstructionItemViewModel list) index =
            match images with
            | head :: tail ->
                let isExportTaregt = head.IsExportTarget.Value
                let localIndex = if isExportTaregt then index else 0
                head.UpdateImage(imageSize, localIndex, indexFontSize.Value, modelIndexOffset, pointConverter)
                updateImage tail (if isExportTaregt then index + 1 else index)
            | [] -> ()
        updateImage images 1

    member _.ExportSinglePng(_: obj) =
        dispatcher.Background {
            let imageSize = paperSize.Value + margin.Value * 2
            let modelIndexOffset = float (indexOffset.Value - margin.Value) / float paperSize.Value
            let length = images |> List.sumBy (fun image -> if image.IsExportTarget.Value then 1 else 0)
            let columns = columnCount.Value
            let rows = (length + columns - 1) / columns
            let drawPaper (exporter: VisualExporter) =
                let rec recSelf (images: InstructionItemViewModel list) index =
                    match images with
                    | head :: tail when head.IsExportTarget.Value ->
                        let pointConverter =
                            ViewPointConverter(
                                float paperSize.Value,
                                float -paperSize.Value,
                                float (margin.Value + imageSize * ((index - 1) % columns)),
                                float (paperSize.Value + margin.Value + imageSize * ((index - 1) / columns)))
                        head.DrawTo(index, indexFontSize.Value, modelIndexOffset, pointConverter, exporter)
                        recSelf tail (index + 1)
                    | _ :: tail -> recSelf tail index
                    | [] -> ()
                recSelf images 1
            async {
                match! fileManager.SaveStream("{FileType.Png.FileName}png image", $"*.png") with
                | Some(stream) ->
                    use str = stream
                    VisualExporter.ExportPngToStream(str, imageSize * columns, imageSize * rows, drawPaper)
                | _ -> return ()
            }
            |> Async.RunSynchronously
        }
