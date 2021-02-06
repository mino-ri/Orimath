[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Orimath.Plugins.Workspace

[<RequiresExplicitTypeArguments>]
let getTool<'T when 'T :> ITool> (ws: IWorkspace) =
    ws.Tools
    |> Seq.pick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<RequiresExplicitTypeArguments>]
let trygetTool<'T when 'T :> ITool> (ws: IWorkspace) =
    ws.Tools
    |> Seq.tryPick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<RequiresExplicitTypeArguments>]
let getToolOrDefault<'T when 'T :> ITool> (ws: IWorkspace) : 'T =
    Option.defaultValue (Unchecked.defaultof<'T>) (trygetTool<'T> ws)

[<RequiresExplicitTypeArguments>]
let getEffect<'T when 'T :> IEffect> (ws: IWorkspace) : 'T =
    ws.Effects
    |> Seq.pick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<RequiresExplicitTypeArguments>]
let trygetEffect<'T when 'T :> IEffect> (ws: IWorkspace) : 'T option =
    ws.Effects
    |> Seq.tryPick(fun t ->
        match t with
        | :? 'T as target -> Some(target)
        | _ -> None)

[<RequiresExplicitTypeArguments>]
let getEffectOrDefault<'T when 'T :> IEffect> (ws: IWorkspace) : 'T =
    Option.defaultValue (Unchecked.defaultof<'T>) (trygetEffect<'T> ws)
