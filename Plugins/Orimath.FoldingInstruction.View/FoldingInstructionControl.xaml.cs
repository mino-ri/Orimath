using Orimath.FoldingInstruction.View.ViewModels;
using Orimath.Plugins;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orimath.FoldingInstruction.View
{
    /// <summary>
    /// FoldingInstructionControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Main, typeof(FoldingInstructionViewModel))]
    public partial class FoldingInstructionControl : UserControl
    {
        public FoldingInstructionControl()
        {
            InitializeComponent();
        }
    }
}
