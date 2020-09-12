using System.Windows.Input;

namespace Orimath
{
    public static class Internal
    {
        private static readonly KeyGestureConverter _keyGestureConverter = new KeyGestureConverter();

        public static KeyGesture? ConvertToKeyGesture(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                try
                {
                    return(KeyGesture)_keyGestureConverter.ConvertFromInvariantString(source);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}
