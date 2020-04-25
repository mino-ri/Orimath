namespace Orimath.Plugins
open System

type IToolProvidor =
    abstract member GetTools : workspace: IWorkspace -> seq<ITool>

type IEffectProvidor =
    abstract member GetEffects : workspace: IWorkspace -> seq<IEffect>

type IViewModelProvider =
    abstract member GetViewModel : workspace: IWorkspace * invoker: IUIThreadInvoker * pointConverter: PointConverter -> obj

type ViewPane =
    | Main = 0
    | Menu = 1
    | Side = 2

[<AttributeUsage(AttributeTargets.Class)>]
type OrimathViewAttribute(viewModelType: Type, viewPane: ViewPane) =
    inherit Attribute()
    member __.ViewModelType = viewModelType
    member __.ViewPane = viewPane
