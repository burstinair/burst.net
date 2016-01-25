using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Burst
{
    [LocalConfiguration("LocalConfiguration.config")]
    public class LocalConfiguration<T> : ConfigurationSection
    {
        public static void Reload()
        {
            try
            {
                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = System.IO.Path.Combine(Environment.CurrentDirectory, typeof(T).GetAttribute<LocalConfigurationAttribute>().FileName);

                _config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                try
                {
                    if (_config.Sections["Local"] == null)
                    {
                        _singleton = Activator.CreateInstance<T>();
                        _config.Sections.Add("Local", _singleton as ConfigurationSection);
                    }
                    else
                        _singleton = (T)(_config.Sections["Local"] as Object);
                }
                catch
                {
                    _config.Sections.Remove("Local");
                    _singleton = Activator.CreateInstance<T>();
                    _config.Sections.Add("Local", _singleton as ConfigurationSection);
                }
            }
            catch { }
        }
        public static void Save()
        {
            try
            {
                _config.Save(ConfigurationSaveMode.Full);
            }
            catch { }
        }

        private static Configuration _config;
        private static T _singleton;
        public static T Singleton
        {
            get
            {
                if (_singleton == null)
                    Reload();
                return _singleton;
            }
        }
    }
}
