using System.Collections.Generic;
using TwitchIntegration.UI.Panels;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using TwitchIntegration.Config;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TwitchIntegration.UI
{
    public static class UIManager
    {
        public enum Panels
        {
            Stats,
            Tools,
            Options = 11,
        }

        public enum VerticalAnchor
        {
            Top,
            Bottom
        }

        public static VerticalAnchor NavbarAnchor = VerticalAnchor.Top;

        public static bool Initializing { get; internal set; } = true;

        internal static UIBase UiBase { get; private set; }
        internal static UIBase UiNavbar { get; private set; }
        public static GameObject UIRoot => UiNavbar?.RootObject;
        public static RectTransform UIRootRect { get; private set; }
        public static Canvas UICanvas { get; private set; }

        internal static readonly Dictionary<Panels, TIPanel> UIPanels = new();

        public static RectTransform NavBarRect;
        public static GameObject NavbarTabButtonHolder;
        private static readonly Vector2 NAVBAR_DIMENSIONS = new(1020f, 35f);

        private static ButtonRef closeBtn;
        private static ButtonRef reconnectBtn;

        private static int lastScreenWidth;
        private static int lastScreenHeight;

        public static bool ShowNavbar
        {
            get => UiNavbar.Enabled;
            set
            {
                if (!UIRoot || UiNavbar.Enabled == value)
                    return;

                UniversalUI.SetUIActive("com.x3rt.TwitchIntegration", value);
            }
        }

        // Initialization

        internal static void InitUI()
        {
            UiBase = UniversalUI.RegisterUI<ExplorerUIBase>("com.x3rt.TwitchIntegration-Panels", Update);
            UiNavbar = UniversalUI.RegisterUI<ExplorerUIBase>("com.x3rt.TwitchIntegration", Update);

            UIRootRect = UIRoot.GetComponent<RectTransform>();
            UICanvas = UIRoot.GetComponent<Canvas>();
            
            DisplayManager.Init();

            Display display = DisplayManager.ActiveDisplay;
            lastScreenWidth = display.renderingWidth;
            lastScreenHeight = display.renderingHeight;

            // Create UI.
            CreateNavBar();

            
            UIPanels.Add(Panels.Stats, new StatsPanel(UiBase));
            UIPanels.Add(Panels.Tools, new ToolsPanel(UiBase));
            UIPanels.Add(Panels.Options, new OptionsPanel(UiBase));

            // Failsafe fix, in some games all dropdowns displayed values are blank on startup for some reason.
            foreach (Dropdown dropdown in UIRoot.GetComponentsInChildren<Dropdown>(true))
                dropdown.RefreshShownValue();

            Initializing = false;

            if (ConfigManager.Hide_On_Startup.Value)
                ShowNavbar = false;
        }

        // Main UI Update loop

        public static void Update()
        {
            if (!UIRoot)
                return;

            Display display = DisplayManager.ActiveDisplay;
            if (display.renderingWidth != lastScreenWidth || display.renderingHeight != lastScreenHeight)
                OnScreenDimensionsChanged();
        }

        // Panels

        public static TIPanel GetPanel(Panels panel) => UIPanels[panel];

        public static T GetPanel<T>(Panels panel) where T : TIPanel => (T)UIPanels[panel];

        public static void TogglePanel(Panels panel)
        {
            TIPanel uiPanel = GetPanel(panel);
            SetPanelActive(panel, !uiPanel.Enabled);
        }

        public static void SetPanelActive(Panels panelType, bool active)
        {
            GetPanel(panelType).SetActive(active);
        }

        public static void SetPanelActive(TIPanel panel, bool active)
        {
            panel.SetActive(active);
        }

        // navbar

        public static void SetNavBarAnchor()
        {
            switch (NavbarAnchor)
            {
                case VerticalAnchor.Top:
                    NavBarRect.anchorMin = new Vector2(0.5f, 1f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 1f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 0);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;

                case VerticalAnchor.Bottom:
                    NavBarRect.anchorMin = new Vector2(0.5f, 0f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 0f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 35);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;
            }
        }

        // listeners

        private static void OnScreenDimensionsChanged()
        {
            Display display = DisplayManager.ActiveDisplay;
            lastScreenWidth = display.renderingWidth;
            lastScreenHeight = display.renderingHeight;

            foreach (KeyValuePair<Panels, TIPanel> panel in UIPanels)
            {
                panel.Value.EnsureValidSize();
                panel.Value.EnsureValidPosition();
                panel.Value.Dragger.OnEndResize();
            }
        }

        private static void OnCloseButtonClicked()
        {
            ShowNavbar = false;
        }

        private static void Navbar_Toggle_OnValueChanged(KeyCode val)
        {
            closeBtn.ButtonText.text = val.ToString();
        }


        // UI Construction

        private static void CreateNavBar()
        {
            GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4,
                TextAnchor.MiddleCenter);
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            NavbarAnchor = ConfigManager.Main_Navbar_Anchor.Value;
            SetNavBarAnchor();
            ConfigManager.Main_Navbar_Anchor.OnValueChanged += (VerticalAnchor val) =>
            {
                NavbarAnchor = val;
                SetNavBarAnchor();
            };

            // TwitchIntegration title

            string titleTxt =
                $"<color=#9046ff>Twitch Integration</color> <color=#23f753>by x3rt</color> <i><color=grey>v{Main.Instance.Info.Version}</color></i>";
            Text title = UIFactory.CreateLabel(navbarPanel, "Title", titleTxt, TextAnchor.MiddleCenter, default, true,
                14);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);

            // panel tabs

            NavbarTabButtonHolder = UIFactory.CreateUIObject("NavTabButtonHolder", navbarPanel);
            UIFactory.SetLayoutElement(NavbarTabButtonHolder, minHeight: 25, flexibleHeight: 999, flexibleWidth: 999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(NavbarTabButtonHolder, false, true, true, true, 4, 2, 2, 2,
                2);

            // Time scale widget
            // timeScaleWidget = new(navbarPanel);

            //spacer
            GameObject spacer = UIFactory.CreateUIObject("Spacer", navbarPanel);
            UIFactory.SetLayoutElement(spacer, minWidth: 15);

            
            
            // reconnect twitch button

            reconnectBtn = UIFactory.CreateButton(navbarPanel, "ReconnectButton", "Reconnect");
            UIFactory.SetLayoutElement(reconnectBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 100);
            //purple colors
            RuntimeHelper.SetColorBlock(reconnectBtn.Component, new Color(0.5f, 0.2f, 0.8f), new Color(0.4f, 0.1f, 0.7f), new Color(0.3f,0.1f, 0.6f, 0.6f));
            reconnectBtn.Component.onClick.AddListener(() =>
            {
                TwitchChat? a = GameObject.Find("__app")?.GetComponent<TwitchChat>();
                Object.Destroy(a);

                if (a != null)
                    a.Connect();
                else
                    GameObject.Find("__app")?.AddComponent<TwitchChat>();
            });

            // Hide menu button

            closeBtn = UIFactory.CreateButton(navbarPanel, "CloseButton", ConfigManager.Navbar_Toggle.Value.ToString());
            UIFactory.SetLayoutElement(closeBtn.Component.gameObject, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(closeBtn.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));
            closeBtn.OnClick += OnCloseButtonClicked;
            
            ConfigManager.Navbar_Toggle.OnValueChanged += Navbar_Toggle_OnValueChanged;


            

        }
    }
}