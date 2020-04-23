namespace Orimath.Plugins
open System
open System.Collections.Generic
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

type IWorkspace =
    abstract member Paper : Paper
    abstract member UndoStack : IReadOnlyCollection<Paper>
    abstract member RedoStack : IReadOnlyCollection<Paper>
    abstract member CanUndo : bool
    abstract member CanRedo : bool
    abstract member Tools : IReadOnlyCollection<ITool>
    abstract member Effects : IReadOnlyCollection<IEffect>
    abstract member CurrentTool : ITool with get, set
    abstract member SelectedLayers : Layer[] with get, set
    abstract member SelectedEdges : Edge[] with get, set
    abstract member SelectedPoints : Point[] with get, set
    abstract member SelectedLines : LineSegment[] with get, set
    abstract member ChangeBlockDeclared : bool

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
    [<CLIEvent>]
    abstract member PaperCleared : IWorkspaceEvent<Paper>
    [<CLIEvent>]
    abstract member LayerAdded : IWorkspaceEvent<IReadOnlyCollection<Layer>>
    [<CLIEvent>]
    abstract member LayerRemoved : IWorkspaceEvent<IReadOnlyCollection<Layer>>
    [<CLIEvent>]
    abstract member LayerReplaced : IWorkspaceEvent<Layer>
    [<CLIEvent>]
    abstract member LineAdded : IWorkspaceEvent<IReadOnlyCollection<LineSegment>>
    [<CLIEvent>]
    abstract member LineRemoved : IWorkspaceEvent<IReadOnlyCollection<LineSegment>>
    [<CLIEvent>]
    abstract member PointAdded : IWorkspaceEvent<IReadOnlyCollection<Point>>
    [<CLIEvent>]
    abstract member PointRemoved : IWorkspaceEvent<IReadOnlyCollection<Point>>

    abstract member Undo : unit -> unit
    abstract member Redo : unit -> unit
    abstract member BeginChange : unit -> IDisposable
    abstract member Clear : unit -> unit
    abstract member Clear : paper: Paper -> unit
    abstract member AddLayers : layers: seq<Layer> -> unit
    abstract member RemoveLayers : count: int -> unit
    abstract member ReplaceLayer : before: Layer * after: Layer -> unit
    abstract member AddLines : layer: Layer * lines: seq<Line> -> unit
    abstract member AddLines : layer: Layer * lines: seq<LineSegment> -> unit
    abstract member AddLinesRaw : layer: Layer * lines: seq<Line> -> unit
    abstract member AddLinesRaw : layer: Layer * lines: seq<LineSegment> -> unit
    abstract member RemoveLines : layer: Layer * count: int -> unit
    abstract member AddPoints : layer: Layer * points: seq<Point> -> unit
    abstract member RemovePoints : layer: Layer * count: int -> unit

type IToolProvidor =
    abstract member GetTools : IWorkspace -> seq<ITool>

type IEffectProvidor =
    abstract member GetEffects : IWorkspace -> seq<IEffect>
