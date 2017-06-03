using System;
using System.IO;
using System.Runtime.Serialization;
using ProtoBuf;

namespace PEExplorer.Helpers {
    static class CustomSerializer {

        public static void Save<T>(T obj, string filename) where T : class
        {
            var path = GetPath(filename);

            try
            {
                using (var file = File.Open(path, FileMode.Create))
                    Serializer.Serialize(file, obj);
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex);
            }
        }

        public static T Load<T>(string filename) where T : class
        {
            var path = GetPath(filename);
            if (!File.Exists(path))
                return null;

            try
            {
                using (var file = File.Open(path, FileMode.Open))
                {
                    return file.Length != 0 ? Serializer.Deserialize<T>(file) : null;
                }
            }
            catch (Exception ex)
            {
                App.AppLogger.Error(ex);
                return null;
            }
        }
        private static string GetPath(string filename) {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + @"\PEExplorer";
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "\\" + filename;
            return path;
        }
    }
}

