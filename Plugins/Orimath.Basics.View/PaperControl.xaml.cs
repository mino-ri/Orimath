using Orimath.Basics.View.ViewModels;
using Orimath.Plugins;
using System.Windows.Controls;

namespace Orimath.Basics.View
{
    /// <summary>
    /// PaperControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Main, typeof(WorkspaceViewModel))]
    public partial class PaperControl : UserControl
    {
        public PaperControl()
        {
            InitializeComponent();
        }
    }
}
