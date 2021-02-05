namespace Orimath.Controls
open System.Windows.Markup

[<MarkupExtensionReturnType(typeof<float>)>]
type DoubleExtension(value: float) =
    inherit MarkupExtension()

    member val Value = value with get, set

    override this.ProvideValue(_) = box this.Value

    new() = DoubleExtension(0.0)
