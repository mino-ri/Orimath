namespace Orimath.Basics.View.ViewModels
open System
open System.Collections.Concurrent
open System.ComponentModel.DataAnnotations
open System.Reflection
open System.Threading
open Orimath.Controls
open Orimath.Basics.View
open ApplicativeProperty
open ApplicativeProperty.PropOperators
open Orimath.Plugins


type EnumValueViewModel private (value: Enum, name: string) =
    static let cache = ConcurrentDictionary<Type, EnumValueViewModel[]>()

    member val Value = value
    member val Name = name
    
    override _.ToString() = name

    static member GetValues(ty: Type) =
        cache.GetOrAdd(ty, fun t -> [|
                for field in t.GetFields(BindingFlags.Public ||| BindingFlags.Static) do
                let name =
                    field.GetCustomAttribute<DisplayAttribute>()
                    |> Null.bind (fun att -> att.Name)
                    |> Null.defaultValue field.Name
                EnumValueViewModel(Enum.Parse(t, field.Name) :?> Enum, name)
            |])


[<AbstractClass>]
type SettingItemViewModel(name: string) =
    inherit NotifyPropertyChanged()
    member _.Name = name


type BooleanSettingItem(name: string, prop: DelegationProp<bool>) =
    inherit SettingItemViewModel(name)
    member _.Prop = prop


type Int32SettingItem
    (
        name: string,
        prop: DelegationProp<int>,
        minimum: int,
        maximum: int,
        messenger: IMessenger,
        dispatcher: IDispatcher
    ) =
    inherit SettingItemViewModel(name)
    member _.Minimum = minimum
    member _.Maximum = maximum
    member _.Prop = prop
    member val IncrementCommand = Prop.commands (fun _ -> Prop.incr prop) messenger.RootEnable dispatcher.SyncContext
    member val DecrementCommand = Prop.commands (fun _ -> Prop.decr prop) messenger.RootEnable dispatcher.SyncContext


type DoubleSettingItem(name: string, prop: DelegationProp<float>, minimum: float, maximum: float) =
    inherit SettingItemViewModel(name)
    member _.Minimum = minimum
    member _.Maximum = maximum
    member _.Prop = prop


type StringSettingItem(name: string, prop: DelegationProp<string>) =
    inherit SettingItemViewModel(name)
    member _.Prop = prop


type EnumSettingItem private (name: string, prop: ValueProp<EnumValueViewModel>, choices: EnumValueViewModel[]) =
    inherit SettingItemViewModel(name)
    member _.Prop = prop
    member _.Choices = choices

    static member Create(name: string, prop: IProp<'T :> Enum>, synCcontext: SynchronizationContext) =
        let choices = EnumValueViewModel.GetValues(typeof<'T>)
        let findValue (e: 'T) = choices |> Array.find (fun v -> v.Value.Equals(e))
        let innerProp = ValueProp(findValue prop.Value, synCcontext)
        let disposable = new CompositeDisposable([
            Prop.map findValue prop .->. innerProp
            (innerProp |> Prop.map (fun v -> v.Value :?> 'T)) .->. prop
        ])
        EnumSettingItem(name, innerProp, choices), disposable :> IDisposable
        