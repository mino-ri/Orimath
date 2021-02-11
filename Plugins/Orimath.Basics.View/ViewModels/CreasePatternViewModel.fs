namespace Orimath.Basics.View.ViewModels
open Orimath.Controls
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Combination
open Orimath.Plugins
open Orimath.Basics.View

type CreasePatternLineViewModel(line: LineSegment, pointConverter: IViewPointConverter, color: InstructionColor) =
    inherit NotifyPropertyChanged()
    let (ScreenPoint(x1, y1)) = pointConverter.ModelToView(line.Point1)
    let (ScreenPoint(x2, y2)) = pointConverter.ModelToView(line.Point2)

    member _.X1 = x1
    member _.Y1 = y1
    member _.X2 = x2
    member _.Y2 = y2
    member _.Color = color


type CreasePatternViewModel(paper: IPaperModel, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let pointConverter = ViewPointConverter(128.0, -128.0, -0.5, 127.5)
    let lines = ResettableObservableCollection<CreasePatternLineViewModel>()
    do paper.Layers |> subscribeOnUI dispatcher (fun _ ->
        lines.Reset(seq {
            let layers = paper.Layers |> Seq.toArray
            for i = 0 to layers.Length - 1 do
            for edge in layers.[i].OriginalEdges do
            if edge.Inner then
                let isTopEdge =
                   layers
                   |> Seq.take i
                   |> Seq.collect (fun l -> l.OriginalEdges)
                   |> Seq.forall (fun e -> not e.Inner || edge.Line <>~ e.Line) 
                if isTopEdge then
                    yield CreasePatternLineViewModel(
                        edge.Line,
                        pointConverter,
                        if layers.[i].LayerType = LayerType.BackSide
                        then InstructionColor.Blue
                        else InstructionColor.Red)
            else
                yield CreasePatternLineViewModel(edge.Line, pointConverter, InstructionColor.Black)
        }))
        |> ignore

    member _.Lines = lines
