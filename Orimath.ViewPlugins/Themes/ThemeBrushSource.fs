namespace Orimath.Themes


type ThemeBrushSource
    (normal: string * string * string,
     hovered: string * string * string,
     highlight: string * string * string,
     disabled: string * string * string,
     alternated: string * string * string) =

    member val Normal = normal with get, set
    member val Hovered = hovered with get, set
    member val Highlight = highlight with get, set
    member val Disabled = disabled with get, set
    member val Alternated = alternated with get, set

    new() = ThemeBrushSource(("", "", ""), ("", "", ""), ("", "", ""), ("", "", ""), ("", "", ""))

    static member val Default = ThemeBrushSource()


type ThemeBrushesSource
    (control: ThemeBrushSource,
     workspace: ThemeBrushSource,
     input: ThemeBrushSource,
     scrollBar: ThemeBrushSource) =

    member val Control = control with get, set
    member val Workspace = workspace with get, set
    member val Input = input with get, set
    member val ScrollBar = scrollBar with get, set

    new() =
        ThemeBrushesSource(
            ThemeBrushSource.Default, ThemeBrushSource.Default,
            ThemeBrushSource.Default, ThemeBrushSource.Default)
