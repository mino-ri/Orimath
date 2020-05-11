using System.Windows.Controls;
using Orimath.Basics.View.ViewModels;
using Orimath.Plugins;

namespace Orimath.Basics.View
{
    /// <summary>
    /// ToolListControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Side, typeof(ToolListViewModel))]
    public partial class ToolListControl : UserControl
    {
        public ToolListControl()
        {
            InitializeComponent();
        }
    }
}
