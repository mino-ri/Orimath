namespace Orimath.Controls
open System
open System.Windows
open System.Windows.Markup

type LoadExtension(uri: string) =
    inherit MarkupExtension()
    member val Uri = uri with get, set

    override this.ProvideValue(_) =
        Application.LoadComponent(Uri(this.Uri, UriKind.Relative))

    new() = LoadExtension("")
