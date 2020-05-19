namespace Orimath.Plugins
open System

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

type IFunction =
    abstract member Name : string
    abstract member ShortcutKey : string
    abstract member Icon : System.IO.Stream

type ITool =
    inherit IFunction
    abstract member OnClick : target: OperationTarget * modifier: OperationModifier -> unit
    abstract member BeginDrag : source: OperationTarget * modifier: OperationModifier -> bool
    abstract member DragEnter : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> bool
    abstract member DragLeave : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> bool
    abstract member DragOver : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> bool
    abstract member Drop : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> unit

type IEffect =
    inherit IFunction
    abstract member Execute : unit -> unit
    abstract member CanExecute : unit -> bool
    abstract member MenuHieralchy : string[]
    [<CLIEvent>]
    abstract member CanExecuteChanged : IEvent<EventHandler, EventArgs>
