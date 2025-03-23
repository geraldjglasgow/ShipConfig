using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn;
using ServerSync;

namespace ShipConfig
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Main.ModGuid)]
    public class ShipConfig : BaseUnityPlugin
    {
        const string PluginGUID = "com.ShipConfig";
        const string PluginName = "ShipConfig";
        const string PluginVersion = "1.0.0";
        public static readonly ManualLogSource LOG = BepInEx.Logging.Logger.CreateLogSource(PluginName);

        ConfigSync configSync = new ConfigSync(PluginGUID)
            { DisplayName = PluginName, CurrentVersion = PluginVersion, MinimumRequiredVersion = PluginVersion };

        void Awake()
        {
            LOG.LogInfo("SHIPCONFIG: started loading config");
            ShipConfiguration.Initialize(Config, configSync);
            LOG.LogInfo("SHIPCONFIG: finished loading config");
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony harmony = new Harmony(PluginGUID);
            harmony.PatchAll(assembly);
        }
    }
}