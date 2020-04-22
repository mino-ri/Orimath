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

type IPlugin =
    abstract member UpdateSettings : unit -> unit

type ITool =
    inherit IPlugin
    abstract member ShortcutKey : string
    abstract member OnClick : target: DisplayTarget * point: Point * modifier: OperationModifier -> unit
    abstract member BeginDrag : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragEnter : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragLeave : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragOver : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member Drop : target: DisplayTarget * point: Point * modifier: OperationModifier -> unit

type IEffect =
    inherit IPlugin
    abstract member ShortcutKey : string
    abstract member Execute : unit -> unit
    abstract member CanExecute : unit -> bool
    [<CLIEvent>]
    abstract member CanExecuteChanged : IEvent<EventHandler, EventArgs>

type ISetting = interface end

type IWorkspace =
    abstract member Paper : Paper
    abstract member SelectedLayers : Layer[] with get, set
    abstract member SelectedEdges : Edge[] with get, set
    abstract member SelectedPoints : Point[] with get, set
    abstract member SelectedLines : LineSegment[] with get, set

    [<CLIEvent>]
    abstract member CurrentToolChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedLayersChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedEdgesChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedPointsChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedLinesChanged : IEvent<EventHandler, EventArgs>

type IPluginProvidor = interface end

type IToolProvidor =
    inherit IPluginProvidor
    abstract member GetTools : IWorkspace -> seq<ITool>

type IEffectProvidor =
    inherit IPluginProvidor
    abstract member GetEffects : IWorkspace -> seq<IEffect>
