using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Sssl;

namespace Orimath.IO
{
    public static class Settings
    {
        public static readonly string SettingDirectory = 
            Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Settings");

        public static string GetSettingPath(string fileName) =>
            Path.Combine(SettingDirectory, fileName + ".sssl");

        public static void Save(string fileName, object? obj)
        {
            if (!Directory.Exists(SettingDirectory))
                Directory.CreateDirectory(SettingDirectory);

            if (SsslConverter.Default.TryConvertFrom(obj, out var sssl))
            {
                // todo: リトライ処理などを入れる
                try
                {
                    sssl.Save(GetSettingPath(fileName));
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.ToString());
                }
            }
        }

        [return: MaybeNull]
        public static T Load<T>(string fileName)
        {
            var path = GetSettingPath(fileName);
            if (!File.Exists(path))
                return default;

            try
            {
                var sssl = SsslObject.Load(path);
                return SsslConverter.Default.ConvertTo<T>(sssl);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }

            return default;
        }
    }
}
