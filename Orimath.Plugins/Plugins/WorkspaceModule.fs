[<AutoOpen>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Orimath.Plugins.Workspace
open Orimath.Core

let private tryCastTool<'T when 'T :> ITool> (o: ITool) =
    match o with
    | :? 'T as casted -> Some(casted)
    | _ -> None

let private tryCastEffect<'T when 'T :> IEffect> (o: IEffect) =
    match o with
    | :? 'T as casted -> Some(casted)
    | _ -> None

type IWorkspace with
    member ws.GetTool() : #ITool = Seq.pick tryCastTool ws.Tools

    member ws.TrygetTool() : #ITool option = Seq.tryPick tryCastTool ws.Tools

    member ws.GetEffect() : #IEffect = Seq.pick tryCastEffect ws.Effects

    member ws.TryGetEffect() : #IEffect option = Seq.tryPick tryCastEffect ws.Effects

    member ws.ClearPaper(layers) = ws.Paper.Clear(Paper.create layers)

type OperationModifier with
    member m.IsNone = m = OperationModifier.None

    member m.HasRightButton = m.HasFlag(OperationModifier.RightButton)

    member m.HasShift = m.HasFlag(OperationModifier.Shift)

    member m.HasCtrl = m.HasFlag(OperationModifier.Ctrl)

    member m.HasAlt = m.HasFlag(OperationModifier.Alt)
