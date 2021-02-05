namespace Orimath.ViewModels
open System
open System.Collections.Concurrent
open System.Reflection
open System.ComponentModel.DataAnnotations
open Orimath

type EnumValueViewModel private (value: Enum, name: string) =
    static let cache = ConcurrentDictionary<Type, EnumValueViewModel[]>()

    member val Value = value
    member val Name = name
    
    override _.ToString() = name

    static member GetValues(ty: Type) =
        cache.GetOrAdd(ty, fun t ->
            [| for field in t.GetFields(BindingFlags.Public ||| BindingFlags.Static) do
               let name =
                   field.GetCustomAttribute<DisplayAttribute>()
                   |> Null.bind(fun att -> att.Name)
                   |> Null.defaultValue field.Name
               EnumValueViewModel(Enum.Parse(t, field.Name) :?> Enum, name) |])
