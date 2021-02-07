namespace Orimath.UITest
open Orimath.Plugins
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


type UITestPlugin() =
    let mutable setting = UITestPluginSetting()
    interface IViewPlugin with
        member _.Execute(args) =
            args.Workspace.AddEffect(UITestEffect(args.Messenger, args.Dispatcher, setting));
            args.Messenger.RegisterView(ViewPane.Dialog, typeof<ControlListViewModel>, "/Orimath.UITest;component/UIListControl.xaml")
    
    interface IConfigurablePlugin with
        member _.SettingType = typeof<UITestPluginSetting>
        member _.Setting with get() = box setting and set v = setting <- downcast v
