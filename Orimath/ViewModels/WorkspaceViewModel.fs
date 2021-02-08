namespace Orimath.ViewModels
open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Linq
open System.Windows.Input
open Orimath
open Orimath.Controls
open Orimath.IO
open Orimath.Plugins
open Orimath.Internal
open ApplicativeProperty
open ApplicativeProperty.PropOperators


type ViewDeclaration =
    | ViewType of Type
    | ViewUri of string

type WorkspaceViewModel(workspace: IWorkspace) =
    inherit NotifyPropertyChanged()
    let dispatcher = OrimathDispatcher()
    let effectCommands = Dictionary<IEffect, ICommand>()
    let effectParameterCreator = Dictionary<Type, obj -> obj>()
    let preViewModels = ObservableCollection<obj>()
    let mutable systemEffects = array.Empty()
    let mutable setting = GlobalSetting()
    let mutable initialized = false
    let dialog = Prop.value (null: obj)

    member val ViewDefinitions = Dictionary<Type, ViewPane * ViewDeclaration>()
    member val MainViewModels = ObservableCollection<obj>()
    member val TopViewModels = ObservableCollection<obj>()
    member val SideViewModels = ObservableCollection<obj>()
    member val MenuItems = ObservableCollection<MenuItemViewModel>()
    member val ToolGestures = Dictionary<KeyGesture, ITool>()
    member val ViewSize = 0.0 with get, set
    member val Dialog = Prop.asGet dialog
    member val HasDialog = isNotNull <|. dialog
    member val HasNotDialog = isNull <|. dialog
    member val RootEnable =
        (dispatcher.IsExecuting, dialog) ||> Prop.map2 (fun exec d -> not exec && isNull d)
    member val CloseDialogCommand =
        dialog .|> isNotNull |> Prop.command (fun _ -> dialog.Value <- null)

    member _.Height with get() = setting.Height and set value = setting.Height <- value
    member _.Width with get() = setting.Width and set value = setting.Width <- value
    member _.IsExecuting = dispatcher.IsExecuting

    member private this.GetViewModelCollection(viewModelType: Type) =
        if not initialized then
            preViewModels
        else
            match this.ViewDefinitions.TryGetValue(viewModelType) with
            | BoolSome(pane, _) ->
                match pane with
                | ViewPane.Main -> this.MainViewModels
                | ViewPane.Top -> this.TopViewModels
                | ViewPane.Side -> this.SideViewModels
                | _ -> null
            | BoolNone -> null

    member _.SelectTool(tool: ITool) = workspace.CurrentTool.OnNext(tool)

    member this.LoadSetting() =
        let createViewModel () = box <| PluginSettingViewModel(this, dispatcher)
        setting <- Settings.load Settings.Global |> Option.defaultWith GlobalSetting
        this.ViewSize <- float(setting.ViewSize) * 2.0
        systemEffects <- [|
            new GlobalSettingEffect(setting) :> IEffect
            new PluginSettingEffect(this, dispatcher, createViewModel) :> IEffect
        |]

    member _.SaveSetting() =
        Settings.save Settings.Global setting

    member this.Initialize() =
        let pointConverter =
            new ViewPointConverter(
                float(setting.ViewSize),
                -float(setting.ViewSize),
                float(setting.ViewSize) * 0.5,
                float(setting.ViewSize) * 1.5)
        PluginExecutor.ExecutePlugins({
            Workspace = workspace
            Messenger = this
            Dispatcher = dispatcher
            PointConverter = pointConverter
        })
        for effect in Seq.append workspace.Effects systemEffects do
            effectCommands.[effect] <-
                match effect with
                | :? IParametricEffect as parametric ->
                    (effect.CanExecute .&&. this.RootEnable, dispatcher.SyncContext)
                    ||> Prop.commands (fun _ -> this.OpenDialog(ParametricEffectDialogViewModel(parametric, dispatcher, this, this.GetEffectParameterViewModel)))
                | effect ->
                    (effect.CanExecute .&&. this.RootEnable, dispatcher.SyncContext)
                    ||> Prop.commands (fun _ -> dispatcher.Background.Invoke(effect.Execute))
        for tool in workspace.Tools do
            Internal.convertToKeyGesture(tool.ShortcutKey)
            |> Option.iter (fun gesture -> this.ToolGestures.[gesture] <- tool)
        workspace.Initialize()
        initialized <- true

    member private this.CreateMenu() =
        for effect in Seq.append workspace.Effects systemEffects do
            let mutable targetCollection = this.MenuItems
            for name in effect.MenuHieralchy do
                match targetCollection |> Seq.tryFind (fun x -> x.Name = name) with
                | Some(parent) -> targetCollection <- parent.Children
                | None ->
                    let parent = MenuItemViewModel(name)
                    targetCollection.Add(parent)
                    targetCollection <- parent.Children
            targetCollection.Add(MenuItemViewModel(effect, this))

    member this.LoadViewModels() =
        this.CreateMenu()
        for item in preViewModels do this.AddViewModel(item)
        preViewModels.Clear()

    member this.AddViewModel(viewModel: obj) =
        if isNull viewModel then nullArg (nameof viewModel)
        this.GetViewModelCollection(viewModel.GetType())
        |> Null.iter(fun c -> c.Add(viewModel))

    member this.RemoveViewModel(viewModelType: Type) =
        if isNull viewModelType then nullArg (nameof viewModelType)
        this.GetViewModelCollection(viewModelType)
        |> Null.iter (fun c ->
            for vm in c |> Seq.filter viewModelType.IsInstanceOfType |> Seq.toArray do
                ignore <| c.Remove(vm))

    member this.RemoveViewModel(viewModel: obj) =
        if isNull viewModel then nullArg (nameof viewModel)
        this.GetViewModelCollection(viewModel.GetType()) |> Null.iter (fun c -> c.Add(viewModel))


    member this.RegisterView(viewPane: ViewPane, viewModelType: Type, viewType: Type) =
        this.ViewDefinitions.[viewModelType] <- (viewPane, ViewType(viewType))

    member this.RegisterView(viewPane: ViewPane, viewModelType: Type, viewUri: string) =
        this.ViewDefinitions.[viewModelType] <- (viewPane, ViewUri(viewUri))
        
    member _.SetEffectParameterViewModel<'ViewModel>(mapping) =
        effectParameterCreator.[typeof<'ViewModel>] <- fun p -> mapping (p :?> 'ViewModel)

    member _.GetEffectParameterViewModel(parameter) =
        match effectParameterCreator.TryGetValue(parameter.GetType()) with
        | BoolSome(creator) -> creator parameter
        | BoolNone -> upcast SettingViewModel(parameter, dispatcher)

    member _.OpenDialog(viewModel: obj) =
        if not initialized
        then invalidOp "初期化完了前にダイアログを表示することはできません。"
        else dialog .<- viewModel

    member _.CloseDialog() = dialog .<- null

    member _.GetEffectCommand(effect: IEffect) =
        match effectCommands.TryGetValue(effect) with
        | BoolSome(command) -> command
        | BoolNone -> null

    interface IMessenger with
        member this.AddViewModel(viewModel) = this.AddViewModel(viewModel)
        member this.CloseDialogCommand = this.CloseDialogCommand
        member this.GetEffectCommand(effect) = this.GetEffectCommand(effect)
        member this.RegisterView(viewPane: ViewPane, viewModelType: Type, viewType: Type) = this.RegisterView(viewPane, viewModelType, viewType)
        member this.RegisterView(viewPane: ViewPane, viewModelType: Type, viewUri: string) = this.RegisterView(viewPane, viewModelType, viewUri)
        member this.RemoveViewModel(viewModelType: Type) = this.RemoveViewModel(viewModelType)
        member this.RemoveViewModel(viewModel: obj) = this.RemoveViewModel(viewModel)
        member this.SetEffectParameterViewModel(mapping) = this.SetEffectParameterViewModel(mapping)
        member this.CloseDialog() = this.CloseDialog()
        member this.OpenDialog(viewModel) = this.OpenDialog(viewModel)
