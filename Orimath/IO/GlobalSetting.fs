namespace Orimath.IO
open System
open System.ComponentModel.DataAnnotations

[<ReferenceEquality; NoComparison>]
type GlobalSetting =
    {
        [<Display(Name = "{PaperSize}Paper size")>]
        [<Range(1, 5000)>]
        mutable ViewSize: int
        [<Editable(false)>]
        mutable Height: float
        [<Editable(false)>]
        mutable Width: float
    }
    with

    static member CreateDefault() = { ViewSize = 512; Height = 600.0; Width = 800.0 }

    member this.Clone() = this.MemberwiseClone() :?> GlobalSetting

    interface ICloneable with
        member this.Clone() = this.MemberwiseClone()
