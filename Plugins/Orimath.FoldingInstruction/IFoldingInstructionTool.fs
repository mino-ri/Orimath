namespace Orimath.FoldingInstruction
open Orimath.Plugins

type IFoldingInstructionTool =
    inherit ITool
    abstract member FoldingInstruction : FoldingInstruction
