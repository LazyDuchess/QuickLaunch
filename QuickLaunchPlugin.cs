using BepInEx;
using HarmonyLib;
using System;

namespace QuickLaunch
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    internal class QuickLaunchPlugin : BaseUnityPlugin
    {
        public static QuickLaunchPlugin Instance { get; private set; }
        public QuickLaunchConfig LaunchConfig { get; private set; }
        private void Awake()
        {
            Instance = this;
            try
            {
                var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                harmony.PatchAll();
                LaunchConfig = new QuickLaunchConfig(Config);
                Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} loaded!");
            }
            catch(Exception e)
            {
                Logger.LogError($"Failed to load {PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION}!{Environment.NewLine}{e}");
            }
        }
    }
}
