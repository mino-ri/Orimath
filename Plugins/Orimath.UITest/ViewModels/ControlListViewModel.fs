namespace Orimath.UITest.ViewModels
open System.ComponentModel
open System.Windows.Controls
open Orimath.Plugins
open Orimath.UITest

type ControlListViewModel(messenger: IMessenger, setting: UITestPluginSetting) =
    member val ControlTypes = [|
        for x in typeof<Control>.Assembly.GetTypes() do
        if typeof<Control>.IsAssignableFrom(x) && not x.IsAbstract then
            yield x |]

    member _.ContentText = setting.ContentText

    member _.CloseCommand = messenger.CloseDialogCommand

    interface INotifyPropertyChanged with
        member _.add_PropertyChanged(_) = ()
        member _.remove_PropertyChanged(_) = ()
