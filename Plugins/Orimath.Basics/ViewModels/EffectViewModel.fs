namespace Orimath.Basics.ViewModels
open Mvvm
open Orimath.Plugins

type EffectViewModel(effect: IEffect, messenger: IMessenger) =
    inherit NotifyPropertyChanged()
    member val Command = messenger.GetEffectCommand(effect)
    member __.Name = effect.Name
    member __.ShortcutKey = effect.ShortcutKey
    member this.CanExecute = this.Command.CanExecute(null)
    member this.Execute() = this.Command.Execute(null)
