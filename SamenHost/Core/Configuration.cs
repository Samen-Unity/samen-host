using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SamenHost.Core
{
    /// <summary>
    /// General class for Samen's settings
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The port that samen should run on.
        /// </summary>
        public int? ServerPort { get; set; }
        public bool? EnableAuthentication { get; set; }

        /// <summary>
        /// Apply the default values to the config if missing.
        /// </summary>
        public void ApplyDefaults()
        {
            ServerPort ??= 4041;
            EnableAuthentication ??= true;
        }
        

        /// <summary>
        /// Load the config from a spesific file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Configuration LoadAndSaveDefaults(string path)
        {
            Configuration config;

            if (!File.Exists(path))
            {
                config = new Configuration();
                config.ApplyDefaults();
                Save(path, config);
                return config;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var json = File.ReadAllText(path);
            config = JsonSerializer.Deserialize<Configuration>(json, options) ?? new Configuration();

            var originalConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

            config.ApplyDefaults();

            var updatedConfig = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

            if (originalConfig != updatedConfig)
            {
                Save(path, config);
            }

            return config;
        }

        /// <summary>
        /// Save the current config to a spesific file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="config"></param>
        private static void Save(string path, Configuration config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
