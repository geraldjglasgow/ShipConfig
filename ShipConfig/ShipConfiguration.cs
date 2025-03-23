using System;
using BepInEx.Configuration;
using ServerSync;

namespace ShipConfig
{
    public static class ShipConfiguration
    {
        private static ConfigEntry<bool> serverConfigLocked;
        private static ConfigFile _configFile;
        private static ConfigSync _configSync;
        
        public static void Initialize(ConfigFile configFile, ConfigSync configSync)
        {
            _configFile = configFile;
            _configSync = configSync;
            serverConfigLocked = CreateConfigEntry("General", "Lock Configuration", true, "[Server Only] The configuration is locked and may not be changed by clients once it has been synced from the server. Only valid for server config, will have no effect on clients.");
            configSync.AddLockingConfigEntry(serverConfigLocked);
        }
        
        public static ConfigEntry<T> CreateConfigEntry<T>(
            string group,
            string name,
            T defaultValue,
            string description,
            bool synchronizedSetting = true)
        {
            return CreateConfigEntry(
                group,
                name,
                defaultValue,
                new ConfigDescription(description),
                synchronizedSetting
            );
        }

        private static ConfigEntry<T> CreateConfigEntry<T>(
            string group,
            string name,
            T defaultValue,
            ConfigDescription description,
            bool synchronizedSetting = true)
        {
            if (_configFile == null || _configSync == null)
            {
                throw new Exception(
                    "ValheimShipConfig not initialized. Call Initialize() first in your plugin's Awake().");
            }

            var extendedDesc = new ConfigDescription(
                description.Description +
                (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                description.AcceptableValues,
                description.Tags);

            ConfigEntry<T> configEntry = _configFile.Bind(group, name, defaultValue, extendedDesc);

            SyncedConfigEntry<T> syncedConfigEntry = _configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }
    }
}