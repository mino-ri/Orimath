namespace Orimath.Basics.View.ViewModels
open Orimath.Controls
open Orimath.Plugins
open Orimath.FoldingInstruction
open ApplicativeProperty

type InstructionArrowViewModel(pointConverter: IViewPointConverter) =
    inherit NotifyPropertyChanged()
    member val X1 = Prop.value 0.0
    member val X2 = Prop.value 0.0
    member val Y1 = Prop.value 0.0
    member val Y2 = Prop.value 0.0
    member val StartType = Prop.value ArrowType.None
    member val EndType = Prop.value ArrowType.None
    member val Color = Prop.value InstructionColor.Black
    member val Direction = Prop.value ArrowDirection.Auto

    member this.SetModel(model: InstructionArrow) =
        let p1 = pointConverter.ModelToView(model.Line.Point1)
        let p2 = pointConverter.ModelToView(model.Line.Point2)
        this.X1.Value <- p1.X
        this.Y1.Value <- p1.Y
        this.X2.Value <- p2.X
        this.Y2.Value <- p2.Y
        this.StartType.Value <- model.StartType
        this.EndType.Value <- model.EndType
        this.Color.Value <- model.Color
        this.Direction.Value <- model.Direction

    static member Create(pointConverter, model) =
        let vm = InstructionArrowViewModel(pointConverter)
        vm.SetModel(model)
        vm
