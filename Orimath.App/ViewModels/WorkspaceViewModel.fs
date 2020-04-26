namespace Orimath.ViewModels
open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.Windows.Input
open Mvvm
open Orimath.Plugins

type WorkspaceViewModel(workspace: IWorkspace, invoker: IUIThreadInvoker) as this =
    inherit NotifyPropertyChanged()
    let pointConverter = ScreenPointConverter(512.0, 16.0, 16.0)
    let mutable dialog = null
    let closeDialogCommand = ActionCommand((fun _ -> this.CloseDialog()), (fun _ -> isNull this.Dialog))
    let preViewModels = ObservableCollection<obj>()
    let mutable initialized = false

    member val ViewDefinitions = Dictionary<Type, ViewPane * Type>()
    member val MainViewModels = ObservableCollection<obj>()
    member val TopViewModels = ObservableCollection<obj>()
    member val SideViewModels = ObservableCollection<obj>()
    member this.Dialog
        with get() = dialog
        and set(v) =
            if this.SetValue(&dialog, v) then
                closeDialogCommand.OnCanExecuteChanged()

    member this.GetViewModelList(viewModelType: Type) =
        if initialized then
            let hasKey, (pane, _) = this.ViewDefinitions.TryGetValue(viewModelType)
            if hasKey then
                match pane with
                | ViewPane.Main -> Some(this.MainViewModels)
                | ViewPane.Top -> Some(this.TopViewModels)
                | ViewPane.Side -> Some(this.SideViewModels)
                | _ -> None
            else
                None
        else
            Some(preViewModels)
        
    member this.AddViewModel(viewModel: obj) =
        if isNull viewModel then nullArg "viewModel"
        match this.GetViewModelList(viewModel.GetType()) with
        | Some(lst) -> lst.Add(viewModel)
        | None -> ()

    member this.RemoveViewModel(viewModelType: Type) =
        match this.GetViewModelList(viewModelType) with
        | Some(lst) ->
            lst
            |> Seq.filter viewModelType.IsInstanceOfType
            |> Seq.toArray
            |> Array.iter (lst.Remove >> ignore)
        | None -> ()
        
    member this.RemoveViewModel(viewModel: obj) =
        if isNull viewModel then nullArg "viewModel"
        match this.GetViewModelList(viewModel.GetType()) with
        | Some(lst) -> ignore (lst.Remove(viewModel))
        | None -> ()

    member this.OpenDialog(viewModel: obj) =
        if not initialized then invalidOp "初期化完了前にダイアログを表示することはできません。"
        this.Dialog <- viewModel

    member this.CloseDialog() = this.Dialog <- null

    member this.HasDialog = not (isNull this.Dialog)

    member __.CloseDialogCommand = closeDialogCommand :> ICommand

    member this.Initialize() =
        let viewDefs =
            PluginLoader.executePlugins {
                    Workspace = workspace
                    Messenger = this
                    UIThreadInvoker = invoker
                    PointConverter = pointConverter
                }
        for viewType, att in viewDefs do
            this.ViewDefinitions.[att.ViewModelType] <- (att.Pane, viewType)

        workspace.Initialize()
        initialized <- true

    member this.LoadViewModels() =
        preViewModels |> Seq.iter this.AddViewModel
        preViewModels.Clear()

    interface IMessenger with
        member this.CloseDialog() = this.CloseDialog()
        member this.CloseDialogCommand = this.CloseDialogCommand
        member this.OpenDialog(viewModel) = this.OpenDialog(viewModel)
        member this.RemoveViewModel(viewModel: obj) = this.RemoveViewModel(viewModel)
        member this.RemoveViewModel(viewModelType: Type) = this.RemoveViewModel(viewModelType)
        member this.AddViewModel(viewModel) = this.AddViewModel(viewModel)
