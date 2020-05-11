using System.Linq;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class EffectListViewModel : NotifyPropertyChanged
    {
        private readonly IWorkspace _workspace;
        private readonly IMessenger _messenger;

        private EffectViewModel[]? _effects;
        public EffectViewModel[] Effects => _effects ??= GetEffects();

        public EffectListViewModel(IWorkspace workspace, IMessenger messenger)
        {
            _workspace = workspace;
            _messenger = messenger;
        }

        private EffectViewModel[] GetEffects()
        {
            return _workspace.Effects
                .Select(e => new EffectViewModel(e, _messenger))
                .ToArray();
        }
    }
}
