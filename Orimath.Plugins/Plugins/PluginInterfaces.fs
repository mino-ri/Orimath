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

type WorkspaceEventArgs<'TParent, 'TValue>(parent: 'TParent, value: 'TValue) =
    inherit EventArgs()
    member __.Parent = parent
    member __.Value = value

type WorkspaceEventHandler<'TParent, 'TValue> = delegate of obj * WorkspaceEventArgs<'TParent, 'TValue> -> unit

type IWorkspaceEvent<'TParent, 'TValue> = IEvent<WorkspaceEventHandler<'TParent, 'TValue>, WorkspaceEventArgs<'TParent, 'TValue>>

type IWorkspace =
    abstract member Paper : Paper
    abstract member UndoStack : IReadOnlyCollection<Paper>
    abstract member RedoStack : IReadOnlyCollection<Paper>
    abstract member CanUndo : bool
    abstract member CanRedo : bool
    abstract member Tools : IReadOnlyCollection<ITool>
    abstract member Effects : IReadOnlyCollection<IEffect>
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
    abstract member PaperCleared : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member LayerAdded : IWorkspaceEvent<Paper, IReadOnlyCollection<Layer>>
    [<CLIEvent>]
    abstract member LayerRemoved : IWorkspaceEvent<Paper, IReadOnlyCollection<Layer>>
    [<CLIEvent>]
    abstract member LineAdded : IWorkspaceEvent<Layer, IReadOnlyCollection<LineSegment>>
    [<CLIEvent>]
    abstract member LineRemoved : IWorkspaceEvent<Layer, IReadOnlyCollection<LineSegment>>
    [<CLIEvent>]
    abstract member PointAdded : IWorkspaceEvent<Layer, IReadOnlyCollection<Point>>
    [<CLIEvent>]
    abstract member PointRemoved : IWorkspaceEvent<Layer, IReadOnlyCollection<Point>>

    abstract member Undo : unit -> unit
    abstract member Redo : unit -> unit
    abstract member BeginChange : unit -> IDisposable
    abstract member Clear : unit -> unit
    abstract member Clear : paper: Paper -> unit
    abstract member AddLayers : layers: Layer -> unit
    abstract member AddLines : layer: Layer * lines: seq<Line> -> unit
    abstract member AddLines : layer: Layer * lines: seq<LineSegment> -> unit
    abstract member AddPoints : layer: Layer * points: seq<Point> -> unit
    abstract member RemoveLayers : count: int -> unit
    abstract member RemoveLines : layer: Layer * count: int -> unit
    abstract member RemovePoints : layer: Layer * count: int -> unit

type IToolProvidor =
    abstract member GetTools : IWorkspace -> seq<ITool>

type IEffectProvidor =
    abstract member GetEffects : IWorkspace -> seq<IEffect>
