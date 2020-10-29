module internal Orimath.Basics.InternalModule
open System.Reflection
open Orimath.Core
open Orimath.Plugins

let getIcon iconName =
    Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Basics.Icons." + iconName + ".png")

let transform (workspace: IWorkspace) (matrix: Matrix) (reverse: bool) =
    let getLayerType (lt: LayerType) = if reverse then lt.TurnOver() else lt
    let newLayers =
        workspace.Paper.Layers
        |> Seq.map(fun layer ->
            workspace.CreateLayer(
                layer.Edges |> Seq.map(fun e -> Edge(e.Line * matrix, e.Inner)),
                layer.Lines |> Seq.map(fun l -> l * matrix),
                layer.Points |> Seq.map(fun p -> p * matrix),
                getLayerType layer.LayerType,
                layer.OriginalEdges,
                layer.Matrix * matrix))
    workspace.Paper.Clear(workspace.CreatePaper(if reverse then Seq.rev newLayers else newLayers))
