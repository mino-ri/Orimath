using System.Windows.Input;
using System.Windows.Threading;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.ViewModels
{
    public static class EffectCommand
    {
        public static ICommand Create(IEffect effect, OrimathDispatcher dispatcher, WorkspaceViewModel parent)
        {
            if (effect is IParametricEffect parametric)
            {
                return effect.CanExecute.Zip(parent.RootEnable, (a, b) => a && b)
                    .ToCommand(_ => parent.OpenDialog(new ParametricEffectDialogViewModel(parametric, dispatcher, parent)),
                               dispatcher.SynchronizationContext);
            }
            else
            {
                return parent.RootEnable.Zip(effect.CanExecute, (a, b) => a && b)
                    .ToCommand(_ => dispatcher.OnBackgroundAsync(effect.Execute),
                               dispatcher.SynchronizationContext);
            }
        }
    }
}
