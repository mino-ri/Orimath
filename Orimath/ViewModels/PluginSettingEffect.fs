namespace Orimath.ViewModels
open Orimath.Plugins
open ApplicativeProperty

type PluginSettingEffect
    (messenger: IMessenger,
     dispatcher: IDispatcher,
     createViewModel: unit -> obj
    ) =
    interface IEffect with
        member val MenuHieralchy = [| "設定" |]
        member _.Name = "プラグインの設定"
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = dispatcher.UI { messenger.OpenDialog(createViewModel()) }
