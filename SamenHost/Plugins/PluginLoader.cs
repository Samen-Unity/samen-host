using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SamenHost.Plugins
{
    /// <summary>
    /// Plugin loader Utils class
    /// </summary>
    public class PluginLoader
    {

        /// <summary>
        /// Load all plugins that are in the Plugins folder
        /// </summary>
        public static void LoadPlugins()
        {
            // Create folder is needed
            Directory.CreateDirectory("Plugins");

            string[] pluginFolders = Directory.GetDirectories("Plugins");
            Logging.Log("Plugin Loader", $"Found {pluginFolders.Length} plugins to load...", LogType.INFO);
            activePluginInstances = new List<PluginInstance>();
            foreach (string pluginFolder in pluginFolders)
            {
                LoadPluginFromFolder(pluginFolder);
            }
        }


        private static List<PluginInstance> activePluginInstances;

        /// <summary>
        /// Load a plugin from a spesific folder
        /// </summary>
        /// <param name="folder"></param>
        public static void LoadPluginFromFolder(string folder)
        {
            Logging.Log("Plugin Loader", $"Loading plugin from {folder}...", LogType.INFO);
            if (!Directory.Exists(folder))
            {
                Logging.Log("Plugin Loader", $"Plugin as {folder} is not a valid plugin. (Folder does not exist.)", LogType.ERROR);
                return;
            }

            string configFileContent = File.ReadAllText(Path.Combine(folder, "config.json"));
            PluginConfiguration configuration = JsonSerializer.Deserialize<PluginConfiguration>(configFileContent);

            Logging.Log("Plugin Loader", $"Loading '{configuration.Name}' created by '{configuration.Author}'...", LogType.IMPORTANT);
            string codeFilePath = Path.Combine(folder, configuration.Code);

            // Load DLL
            if (codeFilePath != null)
            {
                Logging.Log("Plugin Loader", $"Loading code file...", LogType.INFO);
                Assembly assembly = Assembly.LoadFrom(codeFilePath);

                if(configuration.Class == null)
                {
                    Logging.Log("Plugin Loader", $"No main class was found for plugin at {folder}. null.", LogType.ERROR);
                    return;
                }

                Type type = assembly.GetType(configuration.Class);
                if (type == null)
                { 
                    Logging.Log("Plugin Loader", $"Main class '{configuration.Class}' was not found in assembly. {folder}", LogType.ERROR);
                    return;
                }

                try
                {
                    Plugin plugin = (Plugin)Activator.CreateInstance(type);
                    PluginInstance pluginInstance = new PluginInstance(plugin, configuration);
                    activePluginInstances.Add(pluginInstance);

                    Logging.Log("Plugin Loader", "Passing control to plugin startup method...", LogType.INFO);
                    pluginInstance.plugin.OnLoad();
                    Logging.Log("Plugin Loader", "Plugin startup finished.", LogType.INFO);
                } catch( Exception e )
                {
                    Logging.Log("Plugin Loader", $"Failed to load main class in {folder} because: {e}", LogType.ERROR);

                    return;
                }
            }

            Logging.Log("Plugin Loader", $"Plugin '{configuration.Name}' loaded!", LogType.IMPORTANT);

        }
    }

    /// <summary>
    /// An instance of a plugin currently loaded
    /// </summary>
    public class PluginInstance
    {
        /// <summary>
        /// Instance of the main plugin class
        /// </summary>
        public Plugin plugin;

        /// <summary>
        /// Configuration linked with the plugin
        /// </summary>
        public PluginConfiguration configuration;

        /// <summary>
        /// An instance of a plugin
        /// </summary>
        /// <param name="plugin">An instance of the main class of the plugin</param>
        /// <param name="configuration">The configuration linked with the plugin</param>
        public PluginInstance(Plugin plugin, PluginConfiguration configuration)
        {
            this.plugin = plugin;
            this.configuration = configuration;
        }
    }

    /// <summary>
    /// A configuration linked with a plugin
    /// </summary>
    public class PluginConfiguration
    {
        /// <summary>
        /// The author of the plugin
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The DLL file where the plugin's main class is stored
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The namespace and class name of the main plugin class
        /// </summary>
        public string Class { get; set; }
    }
}
