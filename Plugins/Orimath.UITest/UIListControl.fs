namespace Orimath.UITest
open System
open System.Windows
open System.Windows.Controls
open System.Windows.Controls.Primitives
open System.Windows.Data
open Orimath.UITest.ViewModels

type UIListControl() =
    inherit UserControl()
    let testData = [|
            {
                Id = 1
                Value = "First"
                Children = [|
                    { Id = 4; Value = "Fourth"; Children = array.Empty() }
                    { Id = 5; Value = "Fifth"; Children = array.Empty() }
                |]
            }
            { Id = 2; Value = "Second"; Children = array.Empty() }
            { Id = 3; Value = "Thrid"; Children = array.Empty() }
        |]

    member this.ListBoxSelectionChanged(_: obj, e: SelectionChangedEventArgs) =
        if e.AddedItems.Count = 1 then
            match e.AddedItems.[0], this.FindName("previewHost") with
            | (:? Type as t), (:? Border as previewHost) ->
                try
                    let mutable ctrl = Activator.CreateInstance(t) :?> Control
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
                    tryDo <| fun (c: ItemsControl) ->
                        c.ItemsSource <- testData
                        if c :? ContextMenu || c:? Menu || c :? TreeView then
                            let temp = HierarchicalDataTemplate()
                            let factory = FrameworkElementFactory(typeof<TextBlock>)
                            factory.SetBinding(TextBlock.TextProperty, Binding())
                            temp.VisualTree <- factory
                            temp.ItemsSource <- Binding("Children")
                            c.ItemTemplate <- temp
                    tryDo <| fun (c: ScrollBar) -> c.Orientation <- Orientation.Horizontal
                    tryDo <| fun (c: ContextMenu) ->
                        let contentControl = ContentControl()
                        contentControl.Content <- "Right Click"
                        contentControl.ContextMenu <- c
                        ctrl <- contentControl
                    previewHost.Child <- ctrl
                with
                | _ ->
                    let textBlock = TextBlock()
                    textBlock.Text <- "Error"
                    previewHost.Child <- textBlock
            | _ -> ()
