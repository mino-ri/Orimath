namespace Orimath.Basics
open System
open Orimath.Core
open Orimath.Plugins
open ApplicativeProperty

type NewPaperType =
    | Square
    | Rectangle of width: float * height: float
    | RegularPolygon of number: int

type NewPaperExecutor(workspace: IWorkspace) =
    member val NewPaperType = NewPaperType.Square with get, set

    member this.NewPaper() =
        match this.NewPaperType with
        | NewPaperType.Square -> workspace.Paper.Clear(workspace.CreatePaper([| workspace.CreateLayerFromSize(1.0, 1.0, LayerType.BackSide) |]))
        | NewPaperType.Rectangle(width, height) ->
            let (w, h) =
                if width >= height
                then 1.0, height / width
                else width / height, 1.0
            workspace.Paper.Clear(workspace.CreatePaper([| workspace.CreateLayerFromSize(w, h, LayerType.BackSide) |]))
        | NewPaperType.RegularPolygon(number) when 3 <= number && number <= 12 ->
            let unit = Math.PI / float number
            let size = 0.5
            let vertexes =
                [ for i in 1..2..(number * 2) -> { X = 0.5 + sin (unit * float i) * size; Y = 0.5 - cos (unit * float i) * size } ]
            workspace.Paper.Clear(workspace.CreatePaper([| workspace.CreateLayerFromPolygon(vertexes, LayerType.BackSide) |]))
        | _ -> ()

    member this.ResetEffect =
        { new IEffect with
            member _.MenuHieralchy = [| "編集" |]
            member _.Name = "すべて削除"
            member _.ShortcutKey = "Ctrl+Delete"
            member _.Icon = InternalModule.getIcon "delete"
            member _.CanExecute = upcast Prop.ctrue
            member _.Execute() = this.NewPaper()
        }

    member this.NewPaperEffect =
        { new IParametricEffect with
            member _.MenuHieralchy = [| "編集" |]
            member _.Name = "新しい紙"
            member _.ShortcutKey = "Ctrl+N"
            member _.Icon = InternalModule.getIcon "new_paper"
            member _.CanExecute = upcast Prop.ctrue
            member _.Execute() = this.NewPaper()
            member _.GetParameter() = this :> obj
        }
