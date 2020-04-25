namespace Orimath.ViewModels
open System
open Orimath.Plugins
open System.Windows.Input

type EffectCommand(effect: IEffect, invoker: IUIThreadInvoker) =
    let canExecuteChanged = Event<EventHandler, EventArgs>()
    do effect.CanExecuteChanged.AddHandler(fun sender args ->
        onUI invoker <| fun () -> canExecuteChanged.Trigger(sender, args))

    interface ICommand with
        member __.CanExecute(_) = effect.CanExecute()
        member __.Execute(_) = runAsync effect.Execute
        [<CLIEvent>]
        member __.CanExecuteChanged = canExecuteChanged.Publish

type EffectViewModel(effect: IEffect, invoker: IUIThreadInvoker) =
    inherit NotifyPropertyChanged()
    member val Command = EffectCommand(effect, invoker)
    member __.Name = effect.Name
    member __.ShortcutKey = effect.ShortcutKey
    member __.CanExecute = effect.CanExecute()
    member __.Execute() = runAsync effect.Execute
