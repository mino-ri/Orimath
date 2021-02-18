namespace Orimath.ViewModels
open Orimath
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

type ParametricEffectDialogViewModel
    (effect: IParametricEffect,
     dispatcher: IDispatcher,
     parent: IMessenger,
     createViewModel: obj -> obj
    ) as this =
    inherit NotifyPropertyChanged()

    member val Header = Language.GetWord(effect.Name)
    member val OkText = Language.GetWord("{Ok}OK")
    member val CancelText = Language.GetWord("{Cancel}Cancel")
    member val Parameter = createViewModel (effect.GetParameter())
    member val ExecuteCommand = Prop.ctrue |> Prop.command this.Execute
    member _.CloseCommand = parent.CloseDialogCommand

    member _.Execute(_: obj) =
        dispatcher.Background {
            effect.Execute()
            dispatcher.UI.Invoke(parent.CloseDialog)
        }
