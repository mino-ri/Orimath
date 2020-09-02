using System.Windows.Controls;
using Orimath.Basics.View.ViewModels;
using Orimath.Plugins;

namespace Orimath.Basics.View
{
    /// <summary>
    /// NewPaperDialogControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Dialog, typeof(NewPaperDialogViewModel))]
    public partial class NewPaperDialogControl : UserControl
    {
        public NewPaperDialogControl()
        {
            InitializeComponent();
        }
    }
}
