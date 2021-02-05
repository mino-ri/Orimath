namespace Orimath.Controls
open System
open System.Windows
open System.Windows.Data

[<ValueConversion(typeof<float>, typeof<CornerRadius>)>]
type HalfCornerRadiusValueConverter() =
    interface IValueConverter with
        member _.Convert(value, _, _, _) = box (CornerRadius(unbox value / 2.0))
            
        member _.ConvertBack(_, _, _, _) = raise (NotImplementedException())


[<ValueConversion(typeof<float>, typeof<float>)>]
type SubtractionValueConverter() =
    member val Amount = 0.0 with get, set

    interface IValueConverter with

        member this.Convert(value, _, _, _) = box (max 0.0 (unbox value - this.Amount))
        
        member _.ConvertBack(_, _, _, _) = raise (NotImplementedException())
