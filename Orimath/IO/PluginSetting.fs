namespace Orimath.IO
open System
open System.Collections.Generic
open Sssl

[<Sealed>]
type PluginSetting() =
    member val PluginOrder = array.Empty<string>() with get, set

    member val Settings = Dictionary<string, SsslObject>() with get, set

    member this.Clone() = this.MemberwiseClone() :?> PluginSetting

    interface ICloneable with
        member this.Clone() = this.MemberwiseClone()
