using Jotunn.Managers;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using ServerSync;
using System.IO;
using System;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace ShipConfig {
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class ShipConfig : BaseUnityPlugin {
        const string pluginGUID = "com.ShipConfig";
        const string pluginName = "ShipConfig";
        const string pluginVersion = "1.0.0";
        public static readonly ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(pluginName);
        private readonly Harmony harmoney = new Harmony(pluginGUID);
        public static string configPath = Path.GetDirectoryName(Paths.BepInExConfigPath) + Path.DirectorySeparatorChar + String.Format("{0}.cfg", pluginName);
        public static string SECTION_SHIP = "Ship";
        public static string HEALTH_SUFFIX = ".Health";
        ConfigSync configSync = new ConfigSync(pluginGUID) { DisplayName = pluginName, CurrentVersion = pluginVersion, MinimumRequiredVersion = pluginVersion };
        private static ConfigEntry<bool> serverConfigLocked;
        private static ConfigEntry<float> bigCargoHealth;
        private static ConfigEntry<float> littleBoatHealth;
        private static ConfigEntry<float> mercantShipHealth;
        private static ConfigEntry<float> warShipHealth;
        private static ConfigEntry<float> skuldelevHealth;
        private static ConfigEntry<float> fishingBoatHealth;
        private static ConfigEntry<float> fishingCanoeHealth;
        private static ConfigEntry<float> cargoHealth;
        private static ConfigEntry<float> karveHealth;
        private static ConfigEntry<float> vikingShipHealth;
        private static ConfigEntry<float> raftHealth;

        void Awake() {
            bigCargoHealth = config("Ship", "BigCargoShip.Health", 1500.0f, "Sets health value of BigCargoShip.");
            littleBoatHealth = config("Ship", "LittleBoat.Health", 1500.0f, "Sets health value of LittleBoat.");
            mercantShipHealth = config("Ship", "MercantShip.Health", 1500.0f, "Sets health value of MercantShip.");
            warShipHealth = config("Ship", "WarShip.Health", 1500.0f, "Sets health value of WarShip.");
            skuldelevHealth = config("Ship", "Skuldelev.Health", 1500.0f, "Sets health value of Skuldelev.");
            fishingBoatHealth = config("Ship", "FishingBoat.Health", 1500.0f, "Sets health value of FishingBoat.");
            fishingCanoeHealth = config("Ship", "FishingCanoe.Health", 1500.0f, "Sets health value of FishingCanoe.");
            cargoHealth = config("Ship", "CargoShip.Health", 1500.0f, "Sets health value of CargoShip.");
            karveHealth = config("Ship", "Karve.Health", 1500.0f, "Sets health value of Karve.");
            vikingShipHealth = config("Ship", "VikingShip.Health", 1500.0f, "Sets health value of VikingShip.");
            raftHealth = config("Ship", "Raft.Health", 1500.0f, "Sets health value of Raft.");
            serverConfigLocked = config("General", "Lock Configuration", true, new ConfigDescription("[Server Only] The configuration is locked and may not be changed by clients once it has been synced from the server. Only valid for server config, will have no effect on clients."));
            configSync.AddLockingConfigEntry(serverConfigLocked);
            logger.LogInfo("loaded config");
            harmoney.PatchAll();
        }

        public static void updateShipHealth() {
            logger.LogInfo("starting updating prefabs");
            logger.LogInfo($"created entry {bigCargoHealth.Definition} with value {bigCargoHealth.Value}");
            logger.LogInfo($"created entry {littleBoatHealth.Definition} with value {littleBoatHealth.Value}");
            logger.LogInfo($"created entry {warShipHealth.Definition} with value {warShipHealth.Value}");
            logger.LogInfo($"created entry {raftHealth.Definition} with value {raftHealth.Value}");

            PrefabManager.Cache.GetPrefab<WearNTear>("BigCargoShip").m_health = bigCargoHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("LittleBoat").m_health = littleBoatHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("MercantShip").m_health = mercantShipHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("WarShip").m_health = warShipHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("Skuldelev").m_health = skuldelevHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("FishingBoat").m_health = fishingBoatHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("FishingCanoe").m_health = fishingCanoeHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("CargoShip").m_health = cargoHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("Karve").m_health = karveHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("VikingShip").m_health = vikingShipHealth.Value;
            PrefabManager.Cache.GetPrefab<WearNTear>("Raft").m_health = raftHealth.Value;
            logger.LogInfo("Finished updating prefabs");
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true) {
            ConfigDescription extendedDescription =
                new ConfigDescription(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true) {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        public class Patch_Player_Awake {
            static void Postfix() {
                if (SceneManager.GetActiveScene().name.Equals("main")) {
                    updateShipHealth();
                }
            }
        }
    }
}
