using System.Linq;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class ToolListViewModel : NotifyPropertyChanged
    {
        private readonly IWorkspace _workspace;
        private readonly IDispatcher _dispatcher;

        private ToolViewModel[]? _tools;
        public ToolViewModel[] Tools => _tools ??= 
            _workspace.Tools.Select(t => new ToolViewModel(t)).ToArray();

        private ToolViewModel? _currentTool;
        public ToolViewModel? CurrentTool
        {
            get => _currentTool ??= Tools.FirstOrDefault(t => t.Source == _workspace.CurrentTool);
            set => _workspace.CurrentTool = value?.Source;
        }

        public ToolListViewModel(IWorkspace workspace, IDispatcher dispatcher)
        {
            _workspace = workspace;
            _dispatcher = dispatcher;
            _workspace.CurrentToolChanged += (_, __) => _dispatcher.OnUIAsync(() =>
            {
                _currentTool = null;
                OnPropertyChanged(nameof(CurrentTool));
            });
        }
    }
}
