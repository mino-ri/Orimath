namespace Orimath.ViewModels
open System.ComponentModel.DataAnnotations
open System.Reflection
open Orimath
open Orimath.Controls
open Orimath.Plugins
open Orimath.Internal

type SettingViewModel(object: obj, dispatcher: IDispatcher) =
    let mutable loaded = false
    let items = ResettableObservableCollection<SettingItemViewModel>()

    member this.Items =
        if not loaded then this.LoadItems()
        items

    member private _.LoadItems() =
        loaded <- true
        dispatcher.Background {
            let result = [|
                for prop in object.GetType().GetProperties(BindingFlags.Public ||| BindingFlags.Instance) do
                if isNotNull (prop.GetGetMethod()) && isNotNull (prop.GetSetMethod()) &&
                    (prop.GetCustomAttribute<EditableAttribute>()
                    |> Null.mapv(fun att -> att.AllowEdit)
                    |> Option.defaultValue true) then
                    let propertyType = prop.PropertyType
                    if propertyType = typeof<float> then
                        yield DoubleSettingItemViewModel(prop, object) :> SettingItemViewModel
                    elif propertyType = typeof<int> then
                        yield upcast Int32SettingItemViewModel(prop, object)
                    elif propertyType = typeof<bool> then
                        yield upcast BooleanSettingItemViewModel(prop, object)
                    elif propertyType = typeof<string> then
                        yield upcast StringSettingItemViewModel(prop, object)
                    elif propertyType.IsEnum then
                        yield upcast EnumSettingItemViewModel(prop, object) |]
            do! dispatcher.UI
            items.Reset(result)
        }
