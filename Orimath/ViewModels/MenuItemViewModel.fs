namespace Orimath.ViewModels
open System.Collections.ObjectModel
open System.IO
open System.Windows.Input
open Orimath
open Orimath.Plugins
open Orimath.Controls

type MenuItemViewModel
    (name: string,
     shortcutKey: KeyGesture,
     shortcutKeyText: string,
     command: ICommand,
     iconStream: Stream
    ) =
    inherit NotifyPropertyChanged()
    
    member val Children = ObservableCollection<MenuItemViewModel>()
    member _.Name = name
    member _.ShortcutKey = shortcutKey
    member _.ShortcutKeyText = shortcutKeyText
    member _.Command = command
    member _.IconStream = iconStream

    new(name: string) = MenuItemViewModel(name, null, "", null, null)
    
    new(name: string, command: ICommand) = MenuItemViewModel(name, null, "", command, null)

    new(effect: IEffect, messenger: IMessenger) =
        let shortcutKey, shortcutKeyText =
            match Internal.convertToKeyGesture effect.ShortcutKey with
            | Some(sk) -> sk, effect.ShortcutKey
            | None -> null, ""
        MenuItemViewModel(effect.Name, shortcutKey, shortcutKeyText,
            messenger.GetEffectCommand(effect), effect.Icon)
