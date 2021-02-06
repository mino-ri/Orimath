module internal Orimath.Basics.Internal
open System.Reflection
open Orimath.Core
open Orimath.Plugins

let getIcon iconName =
    Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Basics.Icons." + iconName + ".png")

let transform (workspace: IWorkspace) (matrix: Matrix) reverse =
    let getLayerType lt = if reverse then LayerType.turnOver lt else lt
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

type ExistsBuilder() =
    member inline _.Bind(m, f) = Option.exists f m
    member inline _.Bind(m, f) = List.exists f m
    member inline _.Bind(m, f) = Array.exists f m
    member inline _.Bind(m, f) = Seq.exists f m
    member inline _.Return(x: bool) = x

let exists = ExistsBuilder()

let swapWhen cond a b = if cond then b, a else a, b
