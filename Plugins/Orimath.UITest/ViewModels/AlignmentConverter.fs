namespace Orimath.UITest.ViewModels
open System
open System.Windows
open System.Windows.Data

[<ValueConversion(typeof<bool>, typeof<VerticalAlignment>)>]
type VerticalAlignmentConverter() =
    interface IValueConverter with
        member _.ConvertBack(_, _, _, _) = raise (new NotImplementedException())

        member _.Convert(value, _, _, _) =
            match value with
            | :? bool as b when b -> box VerticalAlignment.Stretch
            | _ -> box VerticalAlignment.Center

[<ValueConversion(typeof<bool>, typeof<HorizontalAlignment>)>]
type HorizontalAlignmentConverter() =
    interface IValueConverter with
        member _.ConvertBack(_, _, _, _) = raise (new NotImplementedException())

        member _.Convert(value, _, _, _) =
            match value with
            | :? bool as b when b -> box HorizontalAlignment.Stretch
            | _ -> box HorizontalAlignment.Center
