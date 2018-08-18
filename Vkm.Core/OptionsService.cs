using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Vkm.Api.Configurator;
using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Core
{
    public class OptionsService: IOptionsService
    {
        private readonly string _filename;
        private readonly Dictionary<string, IOptions> _savedOptions;

        public OptionsService(string filename)
        {
            _filename = filename;

            _savedOptions = new Dictionary<string, IOptions>();
        }

        public void InitOptions(IEnumerable<IConfigurator> configurators)
        {
            foreach (var configurator in configurators)
                configurator.Configure(this);
            
            var readOptions = ReadOptions(_filename);

            foreach (var readOption in readOptions)
                _savedOptions[readOption.Key] = readOption.Value;
        }

        private Dictionary<string, IOptions> ReadOptions(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(filename, FileMode.Open))
                    {
                        return (Dictionary<string, IOptions>) formatter.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception at options deserialization: {ex.ToString()}");
            }

            return new Dictionary<string, IOptions>();
        }

        private IOptions GetSavedOptions(string id, IOptions emptyOptions, bool forceReplace)
        {
            if (emptyOptions == null)
                return null;

            CheckIfSerializable(emptyOptions);

            var typeName = id + "." + emptyOptions.GetType().Name;
            if (forceReplace || !_savedOptions.TryGetValue(typeName, out var saved))
            {
                saved = emptyOptions;
                _savedOptions[typeName] = saved;
            }

            return saved;
        }

        private void CheckIfSerializable(IOptions options)
        {
            if (options.GetType().GetCustomAttribute(typeof(SerializableAttribute)) == null)
                throw new TypeAccessException($"Options type {options.GetType().FullName} should be Serializable.");
        }

        public void SaveOptions()
        {
            SaveOptions(_filename, _savedOptions);
        }

        public void SetDefaultOptions(Identifier identifier, IOptions options)
        {
            GetSavedOptions(identifier.Value, options, true);
        }

        private void SaveOptions(string filename, Dictionary<string, IOptions> options)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                formatter.Serialize(stream, options);
                stream.Close();
            }
        }

        public void InitEntity(IOptionsProvider optionsProvider)
        {
            var defaultOptions = optionsProvider.GetDefaultOptions();
            var options = GetSavedOptions(optionsProvider.Id.Value, defaultOptions, false);
            optionsProvider.InitOptions(options);
        }
    }
  
}
