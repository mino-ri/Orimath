namespace Orimath.Core
open System.Collections.Generic

type IPaper =
    abstract member Layers : IReadOnlyList<ILayer>
