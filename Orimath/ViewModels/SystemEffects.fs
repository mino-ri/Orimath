namespace Orimath.ViewModels
open System.Diagnostics
open Orimath.IO
open Orimath.Plugins
open ApplicativeProperty
open Orimath.Internal

type GlobalSettingEffect(rootSetting: GlobalSetting) =
    let mutable setting = Unchecked.defaultof<_>

    interface IParametricEffect with
        member val MenuHieralchy = [| "設定" |]
        member _.Name = "環境設定"
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.GetParameter() =
            setting <- rootSetting.Clone()
            box setting
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() =
            if isNotNull (box setting) then
                rootSetting.ViewSize <- setting.ViewSize
                Settings.save Settings.Global rootSetting


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


type HelpEffect() =
    interface IEffect with
        member val MenuHieralchy = [| "ヘルプ" |]
        member _.Name = "ヘルプの表示"
        member _.ShortcutKey = "Ctrl+F1"
        member _.Icon = null
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() =
            // todo: 多言語化対応
            let url = "https://github.com/mino-ri/Orimath/blob/master/Documents/ja/manual.md"
            let startInfo = ProcessStartInfo("cmd", "/c start " + url)
            startInfo.CreateNoWindow <- true
            use __ = Process.Start(startInfo)
            ()


type VersionInfoEffect(messenger: IMessenger, dispatcher: IDispatcher) =
    interface IEffect with
        member val MenuHieralchy = [| "ヘルプ" |]
        member _.Name = "バージョン情報"
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = dispatcher.UI { messenger.OpenDialog(VersionInfoViewModel(messenger)) }
