namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins
open Orimath.Combination
open ApplicativeProperty

type ExtendToolBarViewModel(messenger: IMessenger, workspace: IWorkspace, dispatcher: IDispatcher) as this =
    inherit NotifyPropertyChanged()
    let items = ResettableObservableCollection()
    let disposables = ResizeArray<IDisposable>()
    let buffer = ResizeArray<obj>()
    let _ = workspace.CurrentTool |> Observable.subscribe2 (function
        | :? IExtendTool as tool ->
            for d in disposables do d.Dispose()
            disposables.Clear()
            dispatcher.UI {
                tool.ExtendSettings(this)
                items.Reset(buffer)
                buffer.Clear()
            }
        | _ ->
            for d in disposables do d.Dispose()
            disposables.Clear()
            dispatcher.UI { items.Reset(Array.Empty()) })

    member _.Items = items

    interface IExtendToolWorkspace with
        member _.AddEffect(effect) =
            buffer.Add(EffectViewModel(effect, messenger))

        member _.AddInt32Setting(name, prop, minimum, maximum) =
            let prop =
                prop
                |> Prop.mapBoth id (min maximum >> max minimum)
                |> Prop.fetchBoth dispatcher.SyncContext :?> DelegationProp<int>
            buffer.Add(Int32SettingItem(messenger.LocalizeWord(name), prop, minimum, maximum, messenger, dispatcher))

        member _.AddDoubleSetting(name, prop, maximum, minimum) =
            let prop =
                prop
                |> Prop.mapBoth id (min maximum >> max minimum)
                |> Prop.fetchBoth dispatcher.SyncContext :?> DelegationProp<float>
            buffer.Add(DoubleSettingItem(messenger.LocalizeWord(name), prop, minimum, maximum))

        member _.AddBooleanSetting(name, prop) =
            let prop = prop |> Prop.fetchBoth dispatcher.SyncContext :?> DelegationProp<bool>
            buffer.Add(BooleanSettingItem(messenger.LocalizeWord(name), prop))

        member _.AddStringSetting(name, prop) =
            let prop = prop |> Prop.fetchBoth dispatcher.SyncContext :?> DelegationProp<string>
            buffer.Add(StringSettingItem(messenger.LocalizeWord(name), prop))

        member _.AddEnumSetting(name, prop) =
            let setting, disposable =
                EnumSettingItem.Create(messenger.LocalizeWord(name), prop, dispatcher.SyncContext)
            buffer.Add(setting)
            disposables.Add(disposable)
