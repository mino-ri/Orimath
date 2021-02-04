using System;
using System.Windows;
using System.Windows.Markup;

namespace Orimath.Controls
{
    public class LoadExtension : MarkupExtension
    {
        public string Uri { get; set; }

        public LoadExtension() => Uri = "";

        public LoadExtension(string uri) => Uri = uri;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Application.LoadComponent(new Uri(Uri, UriKind.Relative));
        }
    }
}
