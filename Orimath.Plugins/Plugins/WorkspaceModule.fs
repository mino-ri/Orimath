[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Orimath.Plugins.Workspace

let private tryCastTool<'T when 'T :> ITool> (o: ITool) =
    match o with
    | :? 'T as casted -> Some(casted)
    | _ -> None

let private tryCastEffect<'T when 'T :> IEffect> (o: IEffect) =
    match o with
    | :? 'T as casted -> Some(casted)
    | _ -> None

[<RequiresExplicitTypeArguments>]
let getTool<'T when 'T :> ITool> (ws: IWorkspace) = Seq.pick tryCastTool<'T> ws.Tools

[<RequiresExplicitTypeArguments>]
let trygetTool<'T when 'T :> ITool> (ws: IWorkspace) = Seq.tryPick tryCastTool<'T> ws.Tools

[<RequiresExplicitTypeArguments>]
let getEffect<'T when 'T :> IEffect> (ws: IWorkspace) = Seq.pick tryCastEffect<'T> ws.Effects

[<RequiresExplicitTypeArguments>]
let trygetEffect<'T when 'T :> IEffect> (ws: IWorkspace) = Seq.tryPick tryCastEffect<'T> ws.Effects
