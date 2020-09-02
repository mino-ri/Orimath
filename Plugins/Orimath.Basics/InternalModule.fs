module internal Orimath.Basics.InternalModule

open System.Reflection

let getIcon iconName =
    Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Basics.Icons." + iconName + ".png")