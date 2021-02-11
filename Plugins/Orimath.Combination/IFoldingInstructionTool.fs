namespace Orimath.Combination
open Orimath.Plugins

type IFoldingInstructionTool =
    inherit ITool
    abstract member FoldingInstruction : FoldingInstruction
