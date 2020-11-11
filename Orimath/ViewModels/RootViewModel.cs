using Orimath.Core;

namespace Orimath.ViewModels
{
    public class RootViewModel
    {
        public WorkspaceViewModel Workspace { get; } = new(new Workspace());
    }
}
