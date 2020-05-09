using System;
using System.Collections.Generic;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.FoldingInstruction.View.ViewModels
{
    public class FoldingInstructionViewModel : NotifyPropertyChanged
    {
        private readonly IWorkspace _workspace;
        private readonly IDispatcher _dispatcher;
        private readonly IViewPointConverter _pointConverter;
        private FoldingInstruction? _foldingInstruction;

        public FoldingInstructionViewModel(IWorkspace workspace, IDispatcher dispatcher, IViewPointConverter pointConverter)
        {
            _workspace = workspace;
            _dispatcher = dispatcher;
            _pointConverter = pointConverter;
            workspace.CurrentToolChanged += Workspace_CurrentToolChanged;
        }

        private void Workspace_CurrentToolChanged(object? sender, EventArgs e)
        {
            if (_foldingInstruction is { })
            {
                _foldingInstruction.LinesChanged -= Model_LinesChanged;
                _foldingInstruction.ArrowsChanged -= Model_ArrowsChanged;
            }

            _foldingInstruction = (_workspace.CurrentTool as IFoldingInstructionTool)?.FoldingInstruction;
            if (_foldingInstruction is { })
            {
                _foldingInstruction.LinesChanged += Model_LinesChanged;
                _foldingInstruction.ArrowsChanged += Model_ArrowsChanged;
            }

            Visible = _foldingInstruction is { };
        }

        private void Model_ArrowsChanged(object? sender, EventArgs e)
        {
        }

        private void Model_LinesChanged(object? sender, EventArgs e)
        {
        }

        private bool _visible;
        public bool Visible { get => _visible; set => SetValue(ref _visible, value); }
    }
}
