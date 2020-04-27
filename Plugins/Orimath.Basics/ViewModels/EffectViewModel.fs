namespace Orimath.Basics.ViewModels
open System
open System.Windows.Input
open Mvvm
open Orimath.Plugins

type EffectCommand(effect: IEffect, dispatcher: IDispatcher) =
    let canExecuteChanged = Event<EventHandler, EventArgs>()
    do effect.CanExecuteChanged.AddHandler(fun sender args ->
        ignore (dispatcher.OnUIAsync(fun () -> canExecuteChanged.Trigger(sender, args))))

    interface ICommand with
        member __.CanExecute(_) = effect.CanExecute()
        member __.Execute(_) = ignore (dispatcher.OnBackgroundAsync(effect.Execute))
        [<CLIEvent>]
        member __.CanExecuteChanged = canExecuteChanged.Publish

type EffectViewModel(effect: IEffect, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    member val Command = EffectCommand(effect, dispatcher)
    member __.Name = effect.Name
    member __.ShortcutKey = effect.ShortcutKey
    member __.CanExecute = effect.CanExecute()
    member __.Execute() = ignore (dispatcher.OnBackgroundAsync(effect.Execute))
