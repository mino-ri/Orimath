using System.Windows.Controls;
using Orimath.Basics.View.ViewModels;
using Orimath.Plugins;

namespace Orimath.Basics.View
{
    /// <summary>
    /// EffectListControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Top, typeof(EffectListViewModel))]
    public partial class EffectListControl : UserControl
    {
        public EffectListControl()
        {
            InitializeComponent();
        }
    }
}
