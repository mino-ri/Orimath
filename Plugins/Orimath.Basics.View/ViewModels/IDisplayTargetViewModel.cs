using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public interface IDisplayTargetViewModel
    {
        (ILayerModel, DisplayTarget) GetTarget();
    }
}
