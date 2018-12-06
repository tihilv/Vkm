﻿using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace HarmonyHub.Utils
{
    /// <summary>
    ///     Allow serialization into JSON.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        ///     Deserialize the JSON to an object instance
        /// </summary>
        /// <param name="aData">Data returned from Harmony</param>
        /// <returns>instance of T</returns>
        public static T FromJson<T>(string aData)
        {
            // Do some magic replacement to format Action into proper JSON that can then be internalized as object
            // In fact we need the Action command to send a command to the device and it is not always matching the Function name.
            string patched = aData.Replace("\"{\\\"", "{\"").Replace("\\\"}\"", "\"}").Replace("\\\"", "\"");
            // Proceed with standard internalization
            byte[] byteArray = Encoding.UTF8.GetBytes(patched);            
            using (var stream = new MemoryStream(byteArray))
            {
                var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                });
                return (T) dataContractJsonSerializer.ReadObject(stream);
            }
        }

        /// <summary>
        ///     Serialize the object to JSON
        /// </summary>
        /// <param name="data">Data to send to Harmony</param>
        /// <returns>string</returns>
        public static string ToJson<T>(T data)
        {
            var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings());

            using (var memoryStream = new MemoryStream())
            {
                dataContractJsonSerializer.WriteObject(memoryStream, data);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}