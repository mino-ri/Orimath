namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins
open Orimath.Basics.Folds
open ApplicativeProperty
open ApplicativeProperty.PropOperators


type DragFoldNavigationItemViewModel
    (header: string,
     falseText: string,
     trueText: string,
     value: IGetProp<bool option>) =
    member _.Header = header
    member _.TrueText = trueText
    member _.FalseText = falseText
    member _.Value = value


type DragFoldNavigationViewModel(items: DragFoldNavigationItemViewModel[], onDispose: unit -> unit) =
    inherit NotifyPropertyChanged()
    member val Items = items
    new(messenger: IMessenger, dispatcher: IDispatcher, tool: DragFoldTool) =
        let isThrough = ValueProp<bool option>(None, dispatcher.SyncContext)
        let isFoldBack = ValueProp<bool option>(None, dispatcher.SyncContext)
        let isFrontMost = ValueProp<bool option>(None, dispatcher.SyncContext)
        let isFree = ValueProp<bool option>(None, dispatcher.SyncContext)
        let items = [|
            DragFoldNavigationItemViewModel(
                messenger.LocalizeWord("{basic/DragFoldNavigation.MouseButton}Mouse button"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.Overlap}overlap"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.Through}through"),
                isThrough)
            DragFoldNavigationItemViewModel(
                messenger.LocalizeWord("{basic/DragFoldNavigation.Shift}Shift"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.Crease}crease"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.FoldBack}fold back"),
                isFoldBack)
            DragFoldNavigationItemViewModel(
                messenger.LocalizeWord("{basic/DragFoldNavigation.Ctrl}Ctrl"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.All}all"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.FrontMost}front most"),
                isFrontMost)
            DragFoldNavigationItemViewModel(
                messenger.LocalizeWord("{basic/DragFoldNavigation.Alt}Alt"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.Aligned}aligned"),
                messenger.LocalizeWord("{basic/DragFoldNavigation.Free}free"),
                isFree)
        |]
        let disconnector =
            tool.State |> Observable.subscribe2 (function
                | DragFoldState.Ready ->
                    isThrough .<- None
                    isFoldBack .<- None
                    isFrontMost .<- None
                    isFree .<- None
                | DragFoldState.Dragging(through, foldBack, frontMost, free) ->
                    isThrough .<- Some(through)
                    isFoldBack .<- Some(foldBack)
                    isFrontMost .<- Some(frontMost)
                    isFree .<- Some(free))
        new DragFoldNavigationViewModel(items, fun () -> disconnector.Dispose())

    interface IDisposable with
        member _.Dispose() = onDispose()
