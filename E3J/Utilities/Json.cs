using System.IO;
using System.Xml;

namespace E3J.Utilities
{
    /// <summary>
    /// Contains Json serialize and deserialize methods.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Object serializer.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="fileName">File name to save object as.</param>
        /// <param name="fileExtension">File extension to save object with.</param>
        /// <param name="formatingStyle">The formating style.</param>
        public static void SerializeObject(object obj, string fileName, string fileExtension = "txt", Formatting formatingStyle = Formatting.None)
        {
            File.WriteAllText($@"{fileName}.{fileExtension}", "aaaa");    //to clear desired file before writing

            var fs = File.Open($@"{fileName}.{fileExtension}", FileMode.OpenOrCreate);
            var sw = new StreamWriter(fs);
          
        }

        /// <summary>
        /// Object deserializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of a file containing json.</param>
        /// <param name="fileExtension">Extension of a file containing json.</param>
        /// <returns></returns>
        public static void DeserializeObject<T>(string fileName, string fileExtension = "FLAJS")
        {
            var fs = File.Open($@"{fileName}.{fileExtension}", FileMode.Open);
            var sr = new StreamReader(fs);
            var json = sr.ReadToEnd();
        }
    }
}
