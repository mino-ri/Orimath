[<AutoOpen>]
module Orimath.Basics.View.Internal
open System.Reflection
open Orimath.Plugins
open ApplicativeProperty

let tryCast<'T> (value: obj) =
    match value with
    | :? 'T as v -> Some(v)
    | _ -> None

let iterOf (action: 'T -> unit) (value: obj) =
    match value with
    | :? 'T as x -> action x
    | _ -> ()

let isNotNull x = not (isNull x)

let (|ScreenPoint|) (p: System.Windows.Point) = p.X, p.Y

let subscribeOnUI (dispatcher: IDispatcher) callback observable =
    observable |> Observable.subscribe2 (fun item -> dispatcher.UI { callback item })

let viewPath name = "/Orimath.Basics.View;component/" + name + ".xaml"

let getIcon iconName =
    Assembly.GetExecutingAssembly().GetManifestResourceStream($"Orimath.Basics.View.Icons.%s{iconName}.png")
