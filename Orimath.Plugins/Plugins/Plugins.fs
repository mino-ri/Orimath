namespace Orimath.Plugins
open System
open Orimath.Core

/// ドラッグ操作におけるキー修飾およびマウスボタンを表します。
[<Flags>]
type OperationModifier =
    /// 修飾はありません。
    | None = 0
    /// マウスの左ボタンではなく右ボタンが押されています。
    | RightButton = 1
    /// Shift キーが押されています。
    | Shift = 2
    /// Alt キーが押されています。
    | Alt = 4
    /// Ctrl キーが押されています。
    | Ctrl = 8

type IPlugin =
    abstract member Name : string
    abstract member ShortcutKey : string

type ITool =
    inherit IPlugin
    abstract member OnClick : target: DisplayTarget * point: Point * modifier: OperationModifier -> unit
    abstract member BeginDrag : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragEnter : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragLeave : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragOver : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member Drop : target: DisplayTarget * point: Point * modifier: OperationModifier -> unit

type IEffect =
    inherit IPlugin
    abstract member Execute : unit -> unit
    abstract member CanExecute : unit -> bool
    [<CLIEvent>]
    abstract member CanExecuteChanged : IEvent<EventHandler, EventArgs>

type WorkspaceEventArgs<'TValue>(layerIndex: int, value: 'TValue) =
    inherit EventArgs()
    member __.LayerIndex = layerIndex
    member __.Value = value

type WorkspaceEventHandler<'TValue> = delegate of sender: obj * WorkspaceEventArgs<'TValue> -> unit

type IWorkspaceEvent<'TValue> = IEvent<WorkspaceEventHandler<'TValue>, WorkspaceEventArgs<'TValue>>
