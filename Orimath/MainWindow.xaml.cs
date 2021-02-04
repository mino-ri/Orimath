﻿using Orimath.Controls;
using Orimath.Plugins;
using Orimath.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ScreenPoint = System.Windows.Point;
using ApplicativeProperty;

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

        private void Window_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Window_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Window_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void Window_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void Window_ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is not FrameworkElement source) return;
            var position = source.PointToScreen(new ScreenPoint(0d, source.ActualHeight));
            var dpi = VisualTreeHelper.GetDpi(this);
            SystemCommands.ShowSystemMenu(this, new ScreenPoint(position.X / dpi.DpiScaleX, position.Y / dpi.DpiScaleY));
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Dispatcher.Yield();
            SetIcon();

            await Dispatcher.Yield();

            var viewModel = (WorkspaceViewModel)DataContext;
            await Task.Run(viewModel.Initialize);

            foreach (var (viewModelType, (_, viewDecl)) in viewModel.ViewDefinitions)
            {
                var template = new DataTemplate(viewModelType)
                {
                    VisualTree = viewDecl switch
                    {
                        ViewDeclaration.ViewType(var type) => new FrameworkElementFactory(type),
                        ViewDeclaration.ViewUri(var uri) => FromUri(uri),
                        _ => throw new InvalidOperationException(),
                    },
                };
                Resources.Add(template.DataTemplateKey, template);
            }

            await Dispatcher.Yield();
            viewModel.LoadViewModels();

            foreach (var menuItem in viewModel.MenuItems)
                SetShortcutKey(menuItem);

            var selectToolCommand = viewModel.RootEnable.ToCommand(parameter =>
            {
                if (parameter is ITool tool)
                    viewModel.SelectTool(tool);
            });
            foreach (var (gesture, tool) in viewModel.ToolGestures)
                InputBindings.Add(new KeyBinding(selectToolCommand, gesture) { CommandParameter = tool });

            await Dispatcher.Yield();

            MainScrollViewer.ScrollToVerticalOffset((MainScrollViewer.ExtentHeight - MainScrollViewer.ActualHeight) / 2.0);
            MainScrollViewer.ScrollToHorizontalOffset((MainScrollViewer.ExtentWidth - MainScrollViewer.ActualWidth) / 2.0);

            FrameworkElementFactory FromUri(string uri)
            {
                var factory = new FrameworkElementFactory(typeof(ContentControl));
                factory.SetValue(ContentProperty, new LoadExtension(uri));
                return factory;
            }

            void SetShortcutKey(MenuItemViewModel menuItem)
            {
                if (menuItem.ShortcutKey is { } gesture)
                    InputBindings.Add(new KeyBinding(menuItem.Command, gesture));

                foreach (var child in menuItem.Children)
                    SetShortcutKey(child);
            }
        }

        private void SetIcon()
        {
            if (Template.FindName("IconImage", this) is Image image)
            {
                var decoder = BitmapDecoder.Create(
                    new Uri("pack://application:,,,/Orimath;component/icon_ho.ico"),
                    BitmapCreateOptions.None,
                    BitmapCacheOption.OnLoad);
                var dpi = VisualTreeHelper.GetDpi(this);
                var screenWidth = (int)(16.0 * dpi.DpiScaleX);

                var targetIcon = decoder.Frames
                    .Where(x => x.PixelWidth >= screenWidth)
                    .OrderBy(x => x.PixelWidth)
                    .FirstOrDefault()
                    ?? decoder.Frames
                    .OrderByDescending(x => x.PixelWidth)
                    .First();

                image.Source = targetIcon;
            }
        }
    }
}
