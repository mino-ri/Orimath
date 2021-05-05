namespace Orimath.Basics.View.Controls
open System
open System.Windows.Data
open System.Windows.Media

[<ValueConversion(typeof<bool option>, typeof<Brush>)>]
type BooleanOptionToBrushConverter() =
    member val TrueBrush = Brushes.Black with get, set
    member val FalseBrush = Brushes.Silver with get, set
    member val NoneBrush = Brushes.Gray with get, set
    interface IValueConverter with
        member this.Convert(value, _, _, _) =
            match value :?> bool option with
            | Some(true) -> box this.TrueBrush
            | Some(false) -> box this.FalseBrush
            | None -> box this.NoneBrush

        member _.ConvertBack(_, _, _, _) = raise (NotImplementedException())
