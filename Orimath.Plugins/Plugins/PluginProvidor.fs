namespace Orimath.Plugins

type IToolProvidor =
    abstract member GetTools : IWorkspace -> seq<ITool>

type IEffectProvidor =
    abstract member GetEffects : IWorkspace -> seq<IEffect>
