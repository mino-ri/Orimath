namespace Orimath.Basics.View.ViewModels
open Orimath.Controls
open Orimath.Plugins
open Orimath.Combination
open ApplicativeProperty

type InstructionPointViewModel(pointConverter: IViewPointConverter) =
    inherit NotifyPropertyChanged()
    member val X = Prop.value 0.0
    member val Y = Prop.value 0.0
    member val Color = Prop.value InstructionColor.Black

    member this.SetModel(model: InstructionPoint) =
        let p = pointConverter.ModelToView(model.Point)
        this.X.Value <- p.X
        this.Y.Value <- p.Y
        this.Color.Value <- model.Color

    static member Create(pointConverter, model) =
        let vm = InstructionPointViewModel(pointConverter)
        vm.SetModel(model)
        vm
