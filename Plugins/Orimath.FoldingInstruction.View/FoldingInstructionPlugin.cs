using System;
using System.Collections.Generic;
using Orimath.FoldingInstruction.View.ViewModels;
using Orimath.Plugins;

namespace Orimath.FoldingInstruction.View
{
    public class FoldingInstructionPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new FoldingInstructionViewModel(args.Workspace, args.Dispatcher, args.PointConverter));
        }
    }
}
