using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShipConfig.ValheimClasses
{
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public class Patch_Player_Awake {
        public static string SHIP_GROUP = "Ship";
        
        static void Postfix() {
            if (SceneManager.GetActiveScene().name.Equals("main")) {
                SetAllShipHealth();
            }
        }
        
        public static void SetAllShipHealth()
        {
            ShipConfig.LOG.LogInfo("SHIPCONFIGSHIPCONFIGSHIPCONFIGSHIPCONFIG: Looping through Jötunn PrefabManager cache");
            foreach (KeyValuePair<string, Object> entry in PrefabManager.Cache.GetPrefabs(typeof(Ship)))
            {
                Ship ship = (Ship) entry.Value;
                ShipConfig.LOG.LogInfo($"SHIPCONFIGSHIPCONFIGSHIPCONFIGSHIPCONFIG: found ship with name {ship.name}");
                if (ship != null)
                {
                    WearNTear wnt = ship.GetComponent<WearNTear>();
                    if (wnt != null)
                    {
                        ConfigEntry<float> shipHealth = ShipConfiguration.CreateConfigEntry(SHIP_GROUP, $"{ship.name}.Health", wnt.m_health, $"Sets health value of {ship.name}.");
                        wnt.m_health = shipHealth.Value;
                    }
                }
            }
        }
    }
}