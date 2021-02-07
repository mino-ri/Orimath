namespace Orimath.UITest
open System
open System.Windows
open System.Windows.Controls
open System.Windows.Controls.Primitives
open Orimath.UITest.ViewModels

type UIListControl() =
    inherit UserControl()
    let testData =
        [| { Id = 1; Value = "First" }
           { Id = 2; Value = "Second" }
           { Id = 3; Value = "Thrid" } |]

    let noPreviewTypes = [| typeof<Page>; typeof<Window>; typeof<ToolTip>; typeof<ContextMenu> |]

    member this.ListBox_SelectionChanged(_: obj, e: SelectionChangedEventArgs) =
        if e.AddedItems.Count = 1 then
            match e.AddedItems.[0], this.FindName("previewHost") with
            | (:? Type as t), (:? Border as previewHost) ->
                if t.GetConstructors() |> Array.forall (fun c -> c.GetParameters().Length <> 0) ||
                   noPreviewTypes |> Array.exists (fun np -> np.IsAssignableFrom(t)) then
                    let textBlock = TextBlock()
                    textBlock.Text <- "No Preview"
                    previewHost.Child <- textBlock
                else
                    try
                        let ctrl = Activator.CreateInstance(t) :?> Control
                        let inline tryDo (action: 'T -> unit) =
                            match ctrl with
                            | :? 'T as c -> action c
                            | _ -> ()
                        ctrl.MinWidth <- 120.0
                        ctrl.MinHeight <- 24.0
                        tryDo <| fun (c: ContentControl) ->
                            c.Content <-
                                match this.DataContext with
                                | :? ControlListViewModel as vm -> vm.ContentText
                                | _ -> "Content"
                        tryDo <| fun (c: HeaderedContentControl) -> c.Header <- "Header"
                        tryDo <| fun (c: HeaderedItemsControl) -> c.Header <- "Header"
                        tryDo <| fun (c: RangeBase) -> c.Value <- c.Maximum / 2.0
                        tryDo <| fun (c: ItemsControl) -> c.ItemsSource <- testData
                        tryDo <| fun (c: ScrollBar) -> c.Orientation <- Orientation.Horizontal
                        previewHost.Child <- ctrl
                    with
                    | _ ->
                        let textBlock = TextBlock()
                        textBlock.Text <- "Error"
                        previewHost.Child <- textBlock
            | _ -> ()
