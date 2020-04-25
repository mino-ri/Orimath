namespace Orimath.Plugins
open System.Windows

type ViewPane =
    | Main = 0
    | Menu = 1
    | Side = 2

type IUIProvider =
    abstract member GetViewModel : workspace: IWorkspace * invoker: IUIThreadInvoker * pointConverter: ScreenPointConverter -> UIElement

    abstract member ViewPane : ViewPane
