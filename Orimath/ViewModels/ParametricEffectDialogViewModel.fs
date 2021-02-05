namespace Orimath.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

type ParametricEffectDialogViewModel(effect: IParametricEffect, dispatcher: IDispatcher, parent: IMessenger, createViewModel: obj -> obj) as this =
    inherit NotifyPropertyChanged()

    member val Header = effect.Name
    member val Parameter = createViewModel (effect.GetParameter())
    member val ExecuteCommand = Prop.ctrue |> Prop.command this.Execute
    member _.CloseCommand = parent.CloseDialogCommand

    member _.Execute(_: obj) =
        dispatcher.OnBackgroundAsync(Action(fun () ->
            effect.Execute()
            dispatcher.OnUIAsync(Action(parent.CloseDialog))))
