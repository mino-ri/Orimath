using System.Windows.Controls;
using Orimath.Basics.View.ViewModels;
using Orimath.Plugins;

namespace Orimath.Basics.View
{
    /// <summary>
    /// NetControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Side, typeof(NetViewModel))]
    public partial class NetControl : UserControl
    {
        public NetControl()
        {
            InitializeComponent();
        }
    }
}
