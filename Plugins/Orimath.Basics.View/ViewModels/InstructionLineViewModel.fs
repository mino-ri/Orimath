namespace Orimath.Basics.View.ViewModels
open Orimath.Controls
open Orimath.Plugins
open Orimath.Combination
open ApplicativeProperty

type InstructionLineViewModel(pointConverter: IViewPointConverter) =
    inherit NotifyPropertyChanged()
    member val X1 = Prop.value 0.0
    member val X2 = Prop.value 0.0
    member val Y1 = Prop.value 0.0
    member val Y2 = Prop.value 0.0
    member val Color = Prop.value InstructionColor.Black

    member this.SetModel(model: InstructionLine) =
        let p1 = pointConverter.ModelToView(model.Line.Point1)
        let p2 = pointConverter.ModelToView(model.Line.Point2)
        this.X1.Value <- p1.X
        this.Y1.Value <- p1.Y
        this.X2.Value <- p2.X
        this.Y2.Value <- p2.Y
        this.Color.Value <- model.Color

    static member Create(pointConverter, model) =
        let vm = InstructionLineViewModel(pointConverter)
        vm.SetModel(model)
        vm
