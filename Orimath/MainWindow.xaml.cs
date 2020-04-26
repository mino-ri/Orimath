using Mvvm;
using Orimath.Core;
using Orimath.Plugins;
using Orimath.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Orimath
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Dispatcher.Yield();
            var viewModel = (WorkspaceViewModel)DataContext;
            await Task.Run(viewModel.Initialize);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                Debug.Print("アセンブリ読み込み失敗: " + args.Name);
                return null;
            };

            foreach (var (viewModelType, (_, uiType)) in viewModel.ViewDefinitions)
            {
                var template = new DataTemplate(viewModelType)
                {
                    VisualTree = new FrameworkElementFactory(uiType),
                };
                Resources.Add(template.DataTemplateKey, template);
            }

            await Dispatcher.Yield();
            viewModel.LoadViewModels();
        }
    }
}
