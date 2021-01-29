namespace Orimath.Plugins
open System.Collections.Generic
open Orimath.Core
open ApplicativeProperty

type IWorkspace =
    abstract member Paper : IPaperModel
    abstract member Tools : IReadOnlyList<ITool>
    abstract member Effects : IReadOnlyList<IEffect>
    abstract member CurrentTool : IProp<ITool>
    abstract member AddTool : tool: ITool -> unit
    abstract member AddEffect : effect: IEffect -> unit

    abstract member Initialize : unit -> unit
    abstract member CreatePaper : layers: seq<ILayer> -> IPaper
    abstract member CreateLayer : edges: seq<Edge> * lines: seq<LineSegment> * points: seq<Point> * layerType: LayerType * originalEdges: seq<Edge> * matrix: Matrix -> ILayer
    abstract member CreateLayerFromSize : width: float * height: float * layerType: LayerType -> ILayer
    abstract member CreateLayerFromPolygon : vertexes: seq<Point> * layerType: LayerType -> ILayer
