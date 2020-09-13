using Orimath.Plugins;
using Orimath.UITest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orimath.UITest
{
    /// <summary>
    /// UIListControl.xaml の相互作用ロジック
    /// </summary>
    [View(ViewPane.Dialog, typeof(ControlListViewModel))]
    public partial class UIListControl : UserControl
    {
        private readonly TestData[] _testData = new[]
        {
            new TestData(1, "First"),
            new TestData(2, "Second"),
            new TestData(3, "Thrid"),
        };

        public UIListControl()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] is Type type)
            {
                if (type.GetConstructors().All(c => c.GetParameters().Length != 0) ||
                    type == typeof(Page) || typeof(Window).IsAssignableFrom(type) || type == typeof(ToolTip) || type == typeof(ContextMenu))
                {
                    previewHost.Child = new TextBlock() { Text = "No Preview" };
                }
                else
                {
                    try
                    {
                        var ctrl = (Control)Activator.CreateInstance(type)!;
                        ctrl.MinWidth = 120;
                        ctrl.MinHeight = 24;

                        { if (ctrl is ContentControl c) c.Content = "Content"; }
                        { if (ctrl is HeaderedContentControl c) c.Header = "Header"; }
                        { if (ctrl is HeaderedItemsControl c) c.Header = "Header"; }
                        { if (ctrl is RangeBase c) c.Value = c.Maximum / 2.0; }
                        { if (ctrl is ItemsControl c) c.ItemsSource = _testData; }
                        { if (ctrl is ScrollBar c) c.Orientation = Orientation.Horizontal; }

                        previewHost.Child = ctrl;
                    }
#pragma warning disable CA1031
                    catch
                    {
                        previewHost.Child = new TextBlock() { Text = "Error" };
                    }
#pragma warning restore CA1031
                }
            }
        }
    }
}
