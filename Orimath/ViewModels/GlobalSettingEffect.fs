namespace Orimath.ViewModels
open Orimath.IO
open Orimath.Plugins
open ApplicativeProperty

type GlobalSettingEffect(rootSetting: GlobalSetting) =
    let mutable setting = Unchecked.defaultof<_>

    interface IParametricEffect with
        member val MenuHieralchy = [| "設定" |]
        member _.Name = "環境設定"
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.GetParameter() =
            setting <- rootSetting.Clone() :?> GlobalSetting
            box setting
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() =
            if not (isNull (box setting)) then
                rootSetting.ViewSize <- setting.ViewSize
                Settings.save Settings.Global rootSetting
