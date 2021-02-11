namespace Orimath.Controls
open System
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open System.Windows.Media
open System.Windows.Media.Imaging
open Orimath
open Orimath.Plugins
open Orimath.ViewModels
open ApplicativeProperty

type MainWindow() =
    inherit Window()

    member this.WindowClose(_: obj, _: ExecutedRoutedEventArgs) =
        SystemCommands.CloseWindow(this)

    member this.WindowMinimize(_: obj, _: ExecutedRoutedEventArgs) =
        SystemCommands.MinimizeWindow(this)

    member this.WindowMaximize(_: obj, _: ExecutedRoutedEventArgs) =
        SystemCommands.MaximizeWindow(this)

    member this.WindowRestore(_: obj, _: ExecutedRoutedEventArgs) =
        SystemCommands.RestoreWindow(this)

    member this.WindowShowSystemMenu(_: obj, e: ExecutedRoutedEventArgs) =
        match e.OriginalSource with
        | :? FrameworkElement as source ->
            let position = source.PointToScreen(ScreenPoint(0.0, source.ActualHeight))
            let dpi = VisualTreeHelper.GetDpi(this)
            SystemCommands.ShowSystemMenu(this, ScreenPoint(position.X / dpi.DpiScaleX, position.Y / dpi.DpiScaleY))
        | _ -> ()

    member this.SetIcon() =
        match this.Template.FindName("IconImage", this) with
        | :? Image as image ->
            let decoder =
                BitmapDecoder.Create
                 (Uri("pack://application:,,,/Orimath;component/icon_ho.ico"),
                  BitmapCreateOptions.None,
                  BitmapCacheOption.OnLoad)
            let dpi = VisualTreeHelper.GetDpi(this)
            let screenWidth = int(16.0 * dpi.DpiScaleX)
            let targetIcon =
                decoder.Frames
                 |> Seq.filter(fun x -> x.PixelWidth >= screenWidth)
                 |> Seq.sortBy(fun x -> x.PixelWidth)
                 |> Seq.tryHead
                 |> Option.defaultWith(fun () ->
                    decoder.Frames
                     |> Seq.sortByDescending(fun x -> x.PixelWidth)
                     |> Seq.head)
            image.Source <- targetIcon
        | _ -> ()

    member this.WindowContentRendered(_: obj, _: EventArgs) =
        this.SetIcon()
        let viewModel = this.DataContext :?> WorkspaceViewModel
        let process1() =
            for (KeyValue(viewModelType, (_, viewDecl))) in viewModel.ViewDefinitions do
                let template = DataTemplate(viewModelType)
                template.VisualTree <-
                    match viewDecl with
                        | ViewType(t) -> FrameworkElementFactory(t)
                        | ViewUri(uri) ->
                            let factory = FrameworkElementFactory(typeof<ContentControl>)
                            factory.SetValue(Window.ContentProperty, LoadExtension(uri))
                            factory
                this.Resources.Add(template.DataTemplateKey, template)
        let process2() =
            viewModel.LoadViewModels()
            let rec setShortcutKey(menuItem: MenuItemViewModel) =
                if Internal.isNotNull menuItem.ShortcutKey then
                    ignore (this.InputBindings.Add(KeyBinding(menuItem.Command, menuItem.ShortcutKey)))
                for child in menuItem.Children do
                    setShortcutKey(child)
            for menuItem in viewModel.MenuItems do
                setShortcutKey(menuItem)
            let selectToolCommand = viewModel.RootEnable |> Prop.command(function
                | :? ITool as tool ->
                    viewModel.SelectTool(tool)
                | _ -> ())
            for (KeyValue(gesture, tool)) in viewModel.ToolGestures do
                let keyBinding = KeyBinding(selectToolCommand, gesture)
                keyBinding.CommandParameter <- tool
                ignore (this.InputBindings.Add(keyBinding))
        let process3() =
            let mainScrollViewer = this.FindName("MainScrollViewer") :?> ScrollViewer
            mainScrollViewer.ScrollToVerticalOffset(
                (mainScrollViewer.ExtentHeight - mainScrollViewer.ActualHeight) / 2.0)
            mainScrollViewer.ScrollToHorizontalOffset(
                (mainScrollViewer.ExtentWidth - mainScrollViewer.ActualWidth) / 2.0)

        async {
            viewModel.Initialize()
            do! Async.AwaitTask(this.Dispatcher.InvokeAsync(Action(process1)).Task)
            do! Async.AwaitTask(this.Dispatcher.InvokeAsync(Action(process2)).Task)
            do! Async.AwaitTask(this.Dispatcher.InvokeAsync(Action(process3)).Task)
        } |> Async.Start
