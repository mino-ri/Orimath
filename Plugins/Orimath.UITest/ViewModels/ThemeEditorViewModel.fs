namespace Orimath.UITest.ViewModels
open Orimath.Controls
open Orimath.Themes
open ApplicativeProperty

type BrushSetViewModel(background: string, foreground: string, border: string) =
    inherit NotifyPropertyChanged()
    member val Background = Prop.value background
    member val Foreground = Prop.value foreground
    member val Border = Prop.value border

    member this.ToSerializable() =
        this.Background.Value, this.Foreground.Value, this.Border.Value


type ThemeBrushViewModel(title: string, source: ThemeBrushSource) =
    inherit NotifyPropertyChanged()
    member _.Title = title
    member val Normal = BrushSetViewModel(source.Normal)
    member val Hovered = BrushSetViewModel(source.Hovered)
    member val Highlight = BrushSetViewModel(source.Highlight)
    member val Disabled = BrushSetViewModel(source.Disabled)
    member val Alternated = BrushSetViewModel(source.Alternated)

    member this.ToSerializable() =
        ThemeBrushSource(
            this.Normal.ToSerializable(),
            this.Hovered.ToSerializable(),
            this.Highlight.ToSerializable(),
            this.Disabled.ToSerializable(),
            this.Alternated.ToSerializable())


type ThemeBrushesViewModel(source: ThemeBrushesSource) =
    inherit NotifyPropertyChanged()
    member val Control = ThemeBrushViewModel("Control", source.Control)
    member val Workspace = ThemeBrushViewModel("Workspace", source.Workspace)
    member val Input = ThemeBrushViewModel("Input", source.Input)
    member val ScrollBar = ThemeBrushViewModel("ScrollBar", source.ScrollBar)

    member this.ToSerializable() =
        ThemeBrushesSource(
            this.Control.ToSerializable(),
            this.Workspace.ToSerializable(),
            this.Input.ToSerializable(),
            this.ScrollBar.ToSerializable())
