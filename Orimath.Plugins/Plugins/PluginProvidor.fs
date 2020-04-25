namespace Orimath.Plugins

type IToolProvidor =
    abstract member GetTools : workspace: IWorkspace -> seq<ITool>

type IEffectProvidor =
    abstract member GetEffects : workspace: IWorkspace -> seq<IEffect>
