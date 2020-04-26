namespace Orimath.Plugins
open System

type FastActivator =
    // パフォーマンス改善したら反映
    static member CreateInstance(``type`` : Type) =
        Activator.CreateInstance(``type``)
