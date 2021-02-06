namespace Orimath.Themes
open System.ComponentModel
open System.Runtime.CompilerServices
open System.Windows
open System.Windows.Media
open Orimath

[<AllowNullLiteral; TypeConverter(typeof<BrushSetConverter>)>]
type BrushSet(basedOn: BrushSet, background: Brush, foreground: Brush, border: Brush) =
    let tryFreeze (brush: Brush) =
        if isNotNull brush && brush.CanFreeze && not brush.IsFrozen then
            brush.Freeze()
        brush
    let mutable background = tryFreeze background
    let mutable foreground = tryFreeze foreground
    let mutable border = tryFreeze border
        
    member val BasedOn = basedOn with get, set

    member this.Background
        with get() = background ?|| (this.BasedOn ?|> (fun b -> b.Background))
        and set v = background <- v
        
    member this.Foreground
        with get() = foreground ?|| (this.BasedOn ?|> (fun b -> b.Foreground))
        and set v = foreground <- v
        
    member this.Border
        with get() = border ?|| (this.BasedOn ?|> (fun b -> b.Border))
        and set v = border <- v
    
    member this.ResolveBrushPath(path) =
        match path with
        | nameof this.Background -> this.Background
        | nameof this.Foreground -> this.Foreground
        | nameof this.Border -> this.Border
        | _ -> null

    static member Default = BrushSet(null, null, null, null)


and [<AllowNullLiteral>] ThemeBrush(basedOn: ThemeBrush, normal: BrushSet, hovered: BrushSet, highlight: BrushSet, disabled: BrushSet) =
    let mutable normal = normal
    let mutable hovered = hovered
    let mutable highlight = highlight
    let mutable disabled = disabled

    member val BasedOn = basedOn with get, set

    member this.Normal
        with get() = normal ?|| (this.BasedOn ?|> (fun b -> b.Normal)) ?|| BrushSet.Default
        and set v = normal <- v

    member this.Hovered
        with get() = hovered ?|| (this.BasedOn ?|> (fun b -> b.Hovered)) ?|| BrushSet.Default
        and set v = hovered <- v

    member this.Highlight
        with get() = highlight ?|| (this.BasedOn ?|> (fun b -> b.Highlight)) ?|| BrushSet.Default
        and set v = highlight <- v

    member this.Disabled
        with get() = disabled ?|| (this.BasedOn ?|> (fun b -> b.Disabled)) ?|| BrushSet.Default
        and set v = disabled <- v

    member this.ResolveBrushSetPath(path) =
        match path with
        | nameof this.Normal -> this.Normal
        | nameof this.Hovered -> this.Hovered
        | nameof this.Highlight -> this.Highlight
        | nameof this.Disabled -> this.Disabled
        | _ -> null

    new() = ThemeBrush(null, null, null, null, null)

    static member val Default = ThemeBrush()

    static member GetTheme(o: DependencyObject) = o.GetValue(ThemeBrush.ThemeProperty) :?> ThemeBrush
    static member SetTheme(o: DependencyObject, value: ThemeBrush) = o.SetValue(ThemeBrush.ThemeProperty, box value)
    static member val ThemeProperty =
        DependencyProperty.RegisterAttached("Theme", typeof<ThemeBrush>, typeof<ThemeBrush>,
            FrameworkPropertyMetadata(ThemeBrush(), FrameworkPropertyMetadataOptions.AffectsRender))


and [<Extension; AbstractClass; Sealed>] ThemeBrushes private() =
    static let solid r g b =
        let brush = SolidColorBrush(Color.FromRgb(r, g, b))
        brush.Freeze()
        brush
    static let border = solid 26uy 81uy 53uy
    static let normal = solid 40uy 121uy 80uy
    static let hover = solid 53uy 161uy 107uy
    static let highlight = solid 255uy 153uy 0uy
    static let highlightLight = solid 255uy 215uy 155uy
    static let highlightBorder = solid 168uy 100uy 0uy
    static let disabledForeground = solid 166uy 166uy 166uy
    static let back = solid 243uy 243uy 243uy
    static let hoveredGray = solid 230uy 230uy 230uy

    static member val Control =
        ThemeBrush(null,
            BrushSet(null, normal, Brushes.White, border),
            BrushSet(null, hover, Brushes.White, border),
            BrushSet(null, highlight, Brushes.White, highlightBorder),
            BrushSet(null, disabledForeground, solid 217uy 217uy 217uy, solid 127uy 127uy 127uy))

    static member val CloseButton =
        ThemeBrush(null,
            BrushSet(null, normal, Brushes.White, border),
            BrushSet(null, solid 232uy 17uy 35uy, Brushes.White, border),
            BrushSet(null, highlight, Brushes.White, highlightBorder),
            BrushSet(null, disabledForeground, solid 217uy 217uy 217uy, solid 127uy 127uy 127uy))

    static member val Workspace =
        ThemeBrush(null,
             BrushSet(null, back, Brushes.Black, normal),
             BrushSet(null, hoveredGray, Brushes.Black, hover),
             BrushSet(null, highlightLight, Brushes.Black, highlightBorder),
             BrushSet(null, back, disabledForeground, disabledForeground))

    static member val Input =
        ThemeBrush(null,
            BrushSet(null, Brushes.White, Brushes.Black, normal),
            BrushSet(null, Brushes.White, Brushes.Black, hover),
            BrushSet(null, Brushes.White, Brushes.Black, highlight),
            BrushSet(null, back, disabledForeground, disabledForeground))

    static member val Selector =
        ThemeBrush(null,
            BrushSet(null, Brushes.White, Brushes.Black, normal),
            BrushSet(null, hoveredGray, Brushes.Black, hover),
            BrushSet(null, highlightLight, Brushes.Black, highlightBorder),
            BrushSet(null, back, disabledForeground, disabledForeground))

    static member val ScrollBar =
        ThemeBrush(null,
            BrushSet(null, back, solid 217uy 217uy 217uy, null),
            BrushSet(null, back, solid 190uy 190uy 190uy, null),
            BrushSet(null, back, solid 190uy 190uy 190uy, null),
            BrushSet(null, back, hoveredGray, null))

    static member ResolveThemeBrushPath(path) =
        match path with
        | nameof ThemeBrushes.Control -> ThemeBrushes.Control
        | nameof ThemeBrushes.CloseButton -> ThemeBrushes.CloseButton
        | nameof ThemeBrushes.Workspace -> ThemeBrushes.Workspace
        | nameof ThemeBrushes.Input -> ThemeBrushes.Input
        | nameof ThemeBrushes.Selector -> ThemeBrushes.Selector
        | nameof ThemeBrushes.ScrollBar -> ThemeBrushes.ScrollBar
        | _ -> null


and BrushSetConverter() =
    inherit TypeConverter()

    let (|Splitted|_|) (value: obj) =
        match value with
        | :? string as str ->
            let parts = str.Split('.')
            if parts.Length = 2 then Some(parts.[0], parts.[1]) else None
        | _ -> None

    override _.CanConvertFrom(context, sourceType) =
        sourceType = typeof<string> || base.CanConvertFrom(context, sourceType)

    override _.ConvertFrom(context, culture, value) =
        match value with
        | Splitted(part0, part1) ->
            let brush =
                ThemeBrushes.ResolveThemeBrushPath(part0)
                ?|> (fun t -> t.ResolveBrushSetPath(part1))
            if isNotNull brush
            then box brush
            else base.ConvertFrom(context, culture, value)
        | _ -> base.ConvertFrom(context, culture, value)
