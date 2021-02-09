namespace Orimath.Controls
open System.Windows

type ThemeHelper() =
    static member GetHierarchy(o: DependencyObject) = o.GetValue(ThemeHelper.HierarchyProperty) :?> float
    static member SetHierarchy(o: DependencyObject, value: float) = o.SetValue(ThemeHelper.HierarchyProperty, box value)
    static member val HierarchyProperty =
        DependencyProperty.RegisterAttached(
            "Hierarchy",
            typeof<float>,
            typeof<ThemeHelper>,
            FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits))
