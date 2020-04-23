[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module Orimath.Plugins.Workspace
open System.Runtime.CompilerServices

[<CompiledName("GetTool"); Extension; RequiresExplicitTypeArguments>]
let getTool<'T when 'T :> ITool> (ws: IWorkspace) : 'T =
    ws.Tools
    |> Seq.pick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<CompiledName("TryGetTool"); Extension; RequiresExplicitTypeArguments>]
let trygetTool<'T when 'T :> ITool> (ws: IWorkspace) : 'T option =
    ws.Tools
    |> Seq.tryPick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<CompiledName("GetToolOrDefault"); Extension; RequiresExplicitTypeArguments>]
let getToolOrDefault<'T when 'T :> ITool> (ws: IWorkspace) : 'T =
    Option.defaultValue (Unchecked.defaultof<'T>) (trygetTool<'T> ws)

[<CompiledName("GetEffect"); Extension; RequiresExplicitTypeArguments>]
let getEffect<'T when 'T :> IEffect> (ws: IWorkspace) : 'T =
    ws.Effects
    |> Seq.pick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<CompiledName("TryGetEffect"); Extension; RequiresExplicitTypeArguments>]
let trygetEffect<'T when 'T :> IEffect> (ws: IWorkspace) : 'T option =
    ws.Effects
    |> Seq.tryPick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<CompiledName("GetEffectOrDefault"); Extension; RequiresExplicitTypeArguments>]
let getEffectOrDefault<'T when 'T :> IEffect> (ws: IWorkspace) : 'T =
    Option.defaultValue (Unchecked.defaultof<'T>) (trygetEffect<'T> ws)
