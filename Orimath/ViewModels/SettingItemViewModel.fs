namespace Orimath.ViewModels
open System
open System.Collections.Concurrent
open System.ComponentModel.DataAnnotations
open System.Linq.Expressions
open System.Reflection
open Orimath
open Orimath.Controls
open Orimath.Internal


type internal PropertyAccessor private (prop: PropertyInfo) =
    static let cache = ConcurrentDictionary<PropertyInfo, PropertyAccessor>()
    let value = Expression.Parameter(typeof<obj>, "value")
    let object = Expression.Parameter(typeof<obj>, "object")
    let getValue: Func<obj, obj> =
        object
        |> convert prop.DeclaringType
        |> property prop
        |> convert typeof<obj>
        |> compileLambda [| object |]
    let setValue: Action<obj, obj> =
        object
        |> convert prop.DeclaringType
        |> property prop
        |> assign (convert prop.PropertyType value)
        |> compileLambda [| object; value |]
    
    member _.GetValue(instance) = getValue.Invoke(instance)
    
    member _.SetValue(instance, value) = setValue.Invoke(instance, value)

    static member GetInstance(prop) = cache.GetOrAdd(prop, PropertyAccessor)


[<AbstractClass>]
type SettingItemViewModel(property: PropertyInfo, object: obj) =
    inherit NotifyPropertyChanged()
    let accessor = PropertyAccessor.GetInstance(property)

    member val Name =
        property.GetCustomAttribute<DisplayAttribute>()
        |> Null.bind (fun att -> Language.GetWord(att.Name))
        |> Null.defaultValue property.Name

    member _.PropertyInfo = property

    member _.GetValue() = accessor.GetValue(object) :?> 'T

    member _.SetValue(value: 'T) = accessor.SetValue(object, box value)


[<AbstractClass>]
type RangeSettingItemViewModel<'T>
    (property: PropertyInfo,
     object: obj,
     defaultMinimum: 'T,
     defaultMaximum: 'T
    ) =
    inherit SettingItemViewModel(property, object)
    let hasRange, min, max =
        property.GetCustomAttribute<RangeAttribute>()
        |> Null.mapv (fun att -> true, att.Minimum :?> 'T, att.Maximum :?> 'T)
        |> Option.defaultValue (false, defaultMinimum, defaultMaximum)

    member _.HasRange = hasRange
    member _.Minimum = min
    member _.Maximum = max

    member this.Value
        with get() = this.GetValue() : 'T
        and set(v: 'T) =
            this.SetValue(v)
            this.OnPropertyChanged()


type DoubleSettingItemViewModel(property: PropertyInfo, object: obj) =
    inherit RangeSettingItemViewModel<float>(property, object, Double.MinValue, Double.MaxValue)


type Int32SettingItemViewModel(property: PropertyInfo, object: obj) =
    inherit RangeSettingItemViewModel<int>(property, object, Int32.MinValue, Int32.MaxValue)
  
  
type BooleanSettingItemViewModel(property: PropertyInfo, object: obj) =
    inherit SettingItemViewModel(property, object)

    member this.Value
        with get() = this.GetValue() : bool
        and set(v: bool) =
            this.SetValue(v)
            this.OnPropertyChanged()


type StringSettingItemViewModel(property: PropertyInfo, object: obj) =
    inherit SettingItemViewModel(property, object)

    member this.Value
        with get() = this.GetValue() : string
        and set(v: string) =
            this.SetValue(v)
            this.OnPropertyChanged()

    member val MaxLength =
        property.GetCustomAttribute<StringLengthAttribute>()
        |> Null.mapv (fun att -> att.MaximumLength)
        |> Option.defaultValue 0
    

type EnumSettingItemViewModel(property: PropertyInfo, object: obj) =
    inherit SettingItemViewModel(property, object)

    member val Choices = EnumValueViewModel.GetValues(property.PropertyType)

    member this.Value
        with get() =
            let value = this.GetValue<Enum>()
            this.Choices |> Array.find (fun v -> v.Value.Equals(value))
        and set(v: EnumValueViewModel) =
            this.SetValue(v.Value)
            this.OnPropertyChanged()
