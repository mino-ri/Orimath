namespace Orimath.Basics.View.ViewModels
open Orimath.Controls
open Orimath.Plugins
open Orimath.FoldingInstruction
open Orimath.Basics.View
open ApplicativeProperty

type FoldingInstructionViewModel(workspace: IWorkspace, dispatcher: IDispatcher, pointConverter: IViewPointConverter) =
    inherit NotifyPropertyChanged()
    let mutable disposables = new CompositeDisposable()
    let visible = Prop.value false
    let points = ResettableObservableCollection<InstructionPointViewModel>()
    let lines = ResettableObservableCollection<InstructionLineViewModel>()
    let arrows = ResettableObservableCollection<InstructionArrowViewModel>()
    
    do ignore (workspace.CurrentTool |> Observable.subscribe2(fun tool ->
        disposables.Dispose()
        disposables <- new CompositeDisposable()
        match tool with
        | :? IFoldingInstructionTool as fTool ->
            fTool.FoldingInstruction.Points
            |> subscribeOnUI dispatcher (fun ps ->
                if ps.Length = points.Count
                then (points, ps) ||> Seq.iter2(fun vm m -> vm.SetModel(m))
                else points.Reset(seq { for p in ps -> InstructionPointViewModel.Create(pointConverter, p) }))
            |> disposables.Add
            fTool.FoldingInstruction.Lines
            |> subscribeOnUI dispatcher (fun ls ->
                if ls.Length = lines.Count
                then (lines, ls) ||> Seq.iter2(fun vm m -> vm.SetModel(m))
                else lines.Reset(seq { for l in ls -> InstructionLineViewModel.Create(pointConverter, l) }))
            |> disposables.Add
            fTool.FoldingInstruction.Arrows
            |> subscribeOnUI dispatcher (fun ars ->
                if ars.Length = arrows.Count
                then (arrows, ars) ||> Seq.iter2(fun vm m -> vm.SetModel(m))
                else arrows.Reset(seq { for a in ars -> InstructionArrowViewModel.Create(pointConverter, a) }))
            |> disposables.Add
            dispatcher.UI {
                points.Reset(seq { for p in fTool.FoldingInstruction.Points.Value -> InstructionPointViewModel.Create(pointConverter, p) })
                lines.Reset(seq { for l in fTool.FoldingInstruction.Lines.Value -> InstructionLineViewModel.Create(pointConverter, l) })
                arrows.Reset(seq { for a in fTool.FoldingInstruction.Arrows.Value -> InstructionArrowViewModel.Create(pointConverter, a) })
                visible.Value <- true
            }
        | _ ->
            dispatcher.UI {
                points.Clear()
                lines.Clear()
                arrows.Clear()
                visible.Value <- false
            }))

    member _.Visible = visible
    member _.Points = points
    member _.Lines = lines
    member _.Arrows = arrows
