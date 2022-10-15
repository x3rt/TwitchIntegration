using System.Collections.Generic;
using TwitchIntegration.UI;
using UnityEngine;

namespace TwitchIntegration.Config
{
    public static class ConfigManager
    {
        internal static readonly Dictionary<string, IConfigElement> ConfigElements = new();
        internal static readonly Dictionary<string, IConfigElement> InternalConfigs = new();

        // Each Mod Loader has its own ConfigHandler.
        // See the TwitchIntegration.Loader namespace for the implementations.
        public static ConfigHandler Handler { get; private set; }

        // Actual UE Settings
        public static ConfigElement<KeyCode> Navbar_Toggle;
        public static ConfigElement<KeyCode> TwitchChat_Reconnect;
        public static ConfigElement<bool> Hide_On_Startup;
        public static ConfigElement<float> Startup_Delay_Time;
        public static ConfigElement<int> Target_Display;
        public static ConfigElement<bool> Debug_Mode;
        public static ConfigElement<bool> Log_Unity_Debug;
        public static ConfigElement<UIManager.VerticalAnchor> Main_Navbar_Anchor;

        // internal configs
        internal static InternalConfigHandler InternalHandler { get; private set; }
        internal static readonly Dictionary<UIManager.Panels, ConfigElement<string>> PanelSaveData = new();

        internal static ConfigElement<string> GetPanelSaveData(UIManager.Panels panel)
        {
            if (!PanelSaveData.ContainsKey(panel))
                PanelSaveData.Add(panel, new ConfigElement<string>(panel.ToString(), string.Empty, string.Empty, true));
            return PanelSaveData[panel];
        }

        public static void Init(ConfigHandler configHandler)
        {
            Handler = configHandler;
            Handler.Init();

            InternalHandler = new InternalConfigHandler();
            InternalHandler.Init();

            CreateConfigElements();

            Handler.LoadConfig();
            InternalHandler.LoadConfig();

#if STANDALONE
            Loader.Standalone.ExplorerEditorBehaviour.Instance?.LoadConfigs();
#endif
        }

        internal static void RegisterConfigElement<T>(ConfigElement<T> configElement)
        {
            if (!configElement.IsInternal)
            {
                Handler.RegisterConfigElement(configElement);
                ConfigElements.Add(configElement.Name, configElement);
            }
            else
            {
                InternalHandler.RegisterConfigElement(configElement);
                InternalConfigs.Add(configElement.Name, configElement);
            }
        }

        private static void CreateConfigElements()
        {
            Navbar_Toggle = new("Navbar Toggle",
                "The key to show/hide Navbar element.",
                KeyCode.F7);

            TwitchChat_Reconnect = new("Reconnect Twitch",
                "The key to reset connection to Twitch if there are any issues", 
                KeyCode.F3);

            Hide_On_Startup = new("Hide On Startup",
                "Should the Navbar be hidden on startup?",
                false);

            Startup_Delay_Time = new("Startup Delay Time",
                "The delay on startup before the UI is created.",
                1f);

            Target_Display = new("Target Display",
                "The monitor index for TwitchIntegration to use, if you have multiple. 0 is the default display, 1 is secondary, etc. " +
                "Restart recommended when changing this setting. Make sure your extra monitors are the same resolution as your primary monitor.",
                0);

            Main_Navbar_Anchor = new("Main Navbar Anchor",
                "The vertical anchor of the main TwitchIntegration Navbar, in case you want to move it.",
                UIManager.VerticalAnchor.Bottom);
            
            Debug_Mode = new("Debug Mode",
                "Enables debug mode, which will log more information to the console.",
                false);

            Log_Unity_Debug = new("Log Unity Debug",
                "Should UnityEngine.Debug.Log messages be printed to TwitchIntegration's log?",
                false);
        }
    }
}