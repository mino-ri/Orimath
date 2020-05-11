﻿using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class ToolViewModel : NotifyPropertyChanged
    {
        public ITool Source { get; }

        public string Name => Source.Name;

        public string ShortcutKey => Source.ShortcutKey;

        public string ToolTip => $"{Name} ({ShortcutKey})";

        public ToolViewModel(ITool tool) => Source = tool;
    }
}
