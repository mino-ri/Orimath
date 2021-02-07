namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins

type EffectViewModel(effect: IEffect, messenger: IMessenger) =
    inherit NotifyPropertyChanged()
    member val EffectCommand = messenger.GetEffectCommand(effect)
    member _.Name = effect.Name
    member _.ShortcutKey = effect.ShortcutKey
    member _.IconStream = effect.Icon
    member _.ToolTip =
        if String.IsNullOrWhiteSpace(effect.ShortcutKey)
        then effect.Name
        else $"%s{effect.Name} (%s{effect.ShortcutKey})"


type EffectListViewModel(workspace: IWorkspace, messenger: IMessenger) =
    let lazyEffect =
        lazy([| for e in workspace.Effects -> EffectViewModel(e, messenger) |])

    member _.Effects = lazyEffect.Value
