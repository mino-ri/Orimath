namespace Orimath.Themes
open System.Windows.Markup
open System.Windows.Media

[<MarkupExtensionReturnType(typeof<Brush>)>]
type ThemeBrushExtension(path: string) =
    inherit MarkupExtension()

    member val Path = path with get, set

    override this.ProvideValue(_) =
        let parts = this.Path.Split('.')
        if parts.Length <> 3 then invalidOp "Invalid Path."
        upcast
            ThemeBrushes.ResolveThemeBrushPath(parts[0])
            .ResolveBrushSetPath(parts[1])
            .ResolveBrushPath(parts[2])

    new() = ThemeBrushExtension("")
