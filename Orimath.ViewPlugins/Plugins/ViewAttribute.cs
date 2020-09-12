using System;

namespace Orimath.Plugins
{
    public enum ViewPane
    {
        Main = 0,
        Top = 1,
        Side = 2,
        Dialog = 3,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ViewAttribute : Attribute
    {
        public ViewPane Pane { get; }

        public Type ViewModelType { get; }

        public ViewAttribute(ViewPane pane, Type viewModelType)
        {
            Pane = pane;
            ViewModelType = viewModelType;
        }
    }
}
