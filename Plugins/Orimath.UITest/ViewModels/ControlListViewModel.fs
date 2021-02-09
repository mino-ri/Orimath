namespace Orimath.UITest.ViewModels
open System.ComponentModel
open System.Windows.Controls
open System.Windows.Controls.Primitives
open Orimath.Plugins
open Orimath.UITest

type ControlListViewModel(messenger: IMessenger, setting: UITestPluginSetting) =
    member val ControlTypes = [|
        typeof<Button>
        typeof<CheckBox>
        typeof<ComboBox>
        typeof<ContextMenu>
        typeof<Control>
        typeof<Expander>
        typeof<Frame>
        typeof<GridSplitter>
        typeof<GroupBox>
        typeof<GroupItem>
        typeof<HeaderedContentControl>
        typeof<HeaderedItemsControl>
        typeof<ItemsControl>
        typeof<Label>
        typeof<ListBox>
        typeof<Menu>
        typeof<ProgressBar>
        typeof<RadioButton>
        typeof<RepeatButton>
        typeof<ResizeGrip>
        typeof<ScrollViewer>
        typeof<ScrollBar>
        typeof<Separator>
        typeof<StatusBar>
        typeof<StatusBarItem>
        typeof<Slider>
        typeof<TabControl>
        typeof<TextBox>
        typeof<ToggleButton>
        typeof<ToolBar>
        typeof<TreeView>
    |]

    member _.ContentText = setting.ContentText

    member _.CloseCommand = messenger.CloseDialogCommand

    interface INotifyPropertyChanged with
        member _.add_PropertyChanged(_) = ()
        member _.remove_PropertyChanged(_) = ()
