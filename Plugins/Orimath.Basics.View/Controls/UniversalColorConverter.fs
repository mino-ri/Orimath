namespace Orimath.Basics.View.Controls
open System
open System.Windows.Data
open Orimath.Basics.View
open Orimath.Combination

type UniversalColorConverter() =
    interface IValueConverter with
        member _.Convert(value, _, _, _) =
            let c = value :?> InstructionColor
            box UniversalColor.brushes.[int c |> max 0 |> min 19]

        member _.ConvertBack(_, _, _, _) = raise (NotImplementedException())
