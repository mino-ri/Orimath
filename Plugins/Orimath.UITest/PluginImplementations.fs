namespace Orimath.UITest
open Orimath.Plugins
open Orimath.Themes
open Orimath.UITest.ViewModels
open ApplicativeProperty

type UITestEffect(messenger: IMessenger, dispatcher: IDispatcher, setting: UITestPluginSetting) =
    let viewModel = lazy(ControlListViewModel(messenger, setting))

    interface IEffect with
        member val MenuHieralchy = [| "デバッグ" |]
        member _.Name = "UIテストを開く"
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.CanExecute: IGetProp<bool> = upcast Prop.ctrue
        member _.Execute() = dispatcher.UI { messenger.OpenDialog(viewModel.Value) }


type ThemeEditorEffect(messenger: IMessenger) =
    let mutable viewModel = None
    interface IParametricEffect with
        member val MenuHieralchy = [| "デバッグ" |]
        member _.Name = "テーマカラーの編集"
        member _.ShortcutKey = ""
        member _.Icon = null
        member _.CanExecute: IGetProp<bool> = upcast Prop.ctrue
        member _.GetParameter() =
            let vm = ThemeBrushesViewModel(ThemeBrushes.Instance.ToSerializable())
            viewModel <- Some(vm)
            box vm
        member _.Execute() =
            match viewModel with
            | None -> ()
            | Some(vm) ->
                let src = vm.ToSerializable()
                ThemeBrushes.Instance <- ThemeBrushes.Load(src)
                messenger.SaveSetting("theme", src)


type UITestPlugin() =
    let mutable setting = UITestPluginSetting()
    interface IViewPlugin with
        member _.Execute(args) =
            args.Workspace.AddEffect(UITestEffect(args.Messenger, args.Dispatcher, setting));
            args.Messenger.RegisterView(ViewPane.Dialog, typeof<ControlListViewModel>, "/Orimath.UITest;component/UIListControl.xaml")
    
    interface IConfigurablePlugin with
        member _.SettingType = typeof<UITestPluginSetting>
        member _.Setting with get() = box setting and set v = setting <- downcast v


type ThemeEditorPlugin() =
    interface IViewPlugin with
        member _.Execute(args) =
            args.Workspace.AddEffect(ThemeEditorEffect(args.Messenger));
            args.Messenger.SetEffectParameterViewModel(box: ThemeBrushesViewModel -> obj)
            args.Messenger.RegisterView(ViewPane.Dialog, typeof<ThemeBrushesViewModel>, "/Orimath.UITest;component/ThemeEditorControl.xaml")
    