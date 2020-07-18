using System.Windows.Controls;
using Orimath.Basics.View.ViewModels;
using Orimath.Plugins;

namespace Orimath.Basics.View
{
    /// <summary>
    /// MeasureControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Side, typeof(MeasureViewModel))]
    public partial class MeasureControl : UserControl
    {
        public MeasureControl()
        {
            InitializeComponent();
        }
    }
}
