namespace Orimath.IO
open System
open System.ComponentModel.DataAnnotations

[<Sealed>]
type GlobalSetting() =
    [<Display(Name = "表示サイズ")>]
    [<Range(1, 5000)>]
    member val ViewSize = 512 with get, set

    [<Editable(false)>]
    member val Height = 600.0 with get, set

    [<Editable(false)>]
    member val Width = 800.0 with get, set

    member this.Clone() = this.MemberwiseClone() :?> GlobalSetting

    interface ICloneable with
        member this.Clone() = this.MemberwiseClone()
