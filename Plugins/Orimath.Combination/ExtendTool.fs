namespace Orimath.Combination
open System
open Orimath.Plugins
open ApplicativeProperty

type IExtendToolWorkspace =
    abstract member AddEffect : effect: IEffect -> unit
    abstract member AddInt32Setting : name: string * prop: ValueProp<int> * minimum: int * maximum: int -> unit
    abstract member AddDoubleSetting : name: string * prop: ValueProp<float> * minimum: float * maximum: float -> unit
    abstract member AddBooleanSetting : name: string * prop: ValueProp<bool> -> unit
    abstract member AddStringSetting : name: string * prop: ValueProp<string> -> unit
    abstract member AddEnumSetting<'T when 'T :> Enum> : name: string * prop: ValueProp<'T> -> unit


type IExtendTool =
    abstract member ExtendSettings : ws: IExtendToolWorkspace -> unit
