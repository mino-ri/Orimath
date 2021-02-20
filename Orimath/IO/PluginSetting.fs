namespace Orimath.IO
open System
open System.Collections.Generic
open SsslFSharp

[<ReferenceEquality; NoComparison>]
type PluginSetting =
    {
        mutable PluginOrder: string[]
        mutable Settings: Dictionary<string, Sssl>
    }
    with

    static member CreateDefault() = { PluginOrder = array.Empty(); Settings = Dictionary() }

    member this.Clone() = this.MemberwiseClone() :?> PluginSetting

    interface ICloneable with
        member this.Clone() = this.MemberwiseClone()
