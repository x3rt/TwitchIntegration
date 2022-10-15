using System;
using ManagementScripts;
using SimulationScripts;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;

namespace TwitchIntegration.UI.Panels
{
    public class ToolsPanel : TIPanel
    {
        public override UIManager.Panels PanelType => UIManager.Panels.Tools;
        public override string Name => "Tools";
        public override int MinWidth => 482;
        public override int MinHeight => 164;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override bool ShouldSaveActiveState => true;
        public override bool NavButtonWanted => true;
        public override bool CanDragAndResize => true;


        public ToolsPanel(UIBase owner) : base(owner)
        {
        }

        private Toggle CinematicToggle;
        private InputFieldRef CinematicInterval;
        private InputFieldRef LimitInput;
        private InputFieldRef CapInput;

        protected override void ConstructPanelContent()
        {
            Text cameraLabel = UIFactory.CreateLabel(ContentRoot, "CameraLabel", "Camera", TextAnchor.MiddleCenter);
            UIFactory.SetLayoutElement(cameraLabel.gameObject, minHeight: 30, flexibleWidth: 9999);
            GameObject CinematicRow = UIFactory.CreateHorizontalGroup(this.ContentRoot, "CinematicRow", false, false, true, true, 5, new Vector4(8, 8, 10, 5),
                default, TextAnchor.MiddleLeft);
            GameObject toggleObj = UIFactory.CreateToggle(CinematicRow, "CinematicModeToggle", out CinematicToggle,
                out Text toggleText);
            UIFactory.SetLayoutElement(toggleObj, minHeight: 25, flexibleWidth: 9999);
            CinematicToggle.onValueChanged.AddListener((value) =>
            {
                Main.isCinematic = value;
            });
            CinematicToggle.isOn = Main.isCinematic;
            toggleText.text = "Cinematic Mode";

            
            AddInputField("CinematicInterval", "Auto-swap delay:", "Disabled: 0", out CinematicInterval, (a) =>
                {
                    if (a.Length == 0)
                    {
                        a = "0";
                        Main.cinematicInterval = 0;
                    }
                    if (!ParseUtility.TryParse(a, out int time, out Exception e))
                    {
                        Main.LogWarning("Could not parse value: " + a);
                        CinematicInterval.Text = Main.cinematicInterval.ToString();
                        return;
                    }
                    Main.cinematicInterval = time;
                    
                }, CinematicRow);
            CinematicInterval.Component.textComponent.alignment = TextAnchor.MiddleCenter;
            
            
            Text miscLabel = UIFactory.CreateLabel(ContentRoot, "MiscLabel", "Misc", TextAnchor.MiddleCenter);
            UIFactory.SetLayoutElement(miscLabel.gameObject, minHeight: 30, flexibleWidth: 9999);
            
            GameObject LayRow = UIFactory.CreateHorizontalGroup(this.ContentRoot, "LayRow", false, false, true, true, 5, new Vector4(8, 8, 10, 5),
                default, TextAnchor.MiddleLeft);
            ButtonRef? LayButton = UIFactory.CreateButton(LayRow, "LayButton", "Lay");
            UIFactory.SetLayoutElement(LayButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            LayButton.Component.onClick.AddListener(() =>
            {
                GameObject? t = Tools.GetClosestEntity();
                if (t != null)
                    Commands.lay(t);
            });
            ButtonRef? LayAllButton = UIFactory.CreateButton(LayRow, "LayAllButton", "Lay All");
            UIFactory.SetLayoutElement(LayAllButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            LayAllButton.Component.onClick.AddListener(() =>
            {
                Commands.layAll();
            });
            ButtonRef? LaunchButton = UIFactory.CreateButton(LayRow, "LaunchButton", "Launch");
            UIFactory.SetLayoutElement(LaunchButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            LaunchButton.Component.onClick.AddListener(() =>
            {
                GameObject? a = Tools.GetClosestEntity();
                if (a == null) return;
                Commands.push(a, 10);
            });
            ButtonRef? LaunchAllButton = UIFactory.CreateButton(LayRow, "LaunchAllButton", "Launch All");
            UIFactory.SetLayoutElement(LaunchAllButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            LaunchAllButton.Component.onClick.AddListener(() =>
            {
                Commands.pushAll(10);
            });
            
            // LimitSlider
            GameObject CapLimRow = UIFactory.CreateHorizontalGroup(this.ContentRoot, "LimitRow", false, false, true, true, 5, new Vector4(8, 8, 10, 5),
                default, TextAnchor.MiddleLeft);

            //Slider
            
            
            AddInputField("LimitInput", "Bibite Limit:", "Disabled: 0", out LimitInput, (a) =>
                {
                    if (a.Length == 0)
                    {
                        a = "0";
                        Commands.UpdateBibiteLimit(0);
                    }
                    if (!ParseUtility.TryParse(a, out int l, out Exception e))
                    {
                        Main.LogWarning("Could not parse value: " + a);
                        LimitInput.Text = BibiteSpawner.limitNumber.ToString();
                        return;
                    }

                    Commands.UpdateBibiteLimit(l);

                }, CapLimRow);
            LimitInput.Component.textComponent.alignment = TextAnchor.MiddleCenter;
            
            // CapSlider
            AddInputField("CapInput", "Bibite Cap:", "Disabled: 0", out CapInput, (a) =>
            {
                if (a.Length == 0)
                {
                    a = "0";
                    Commands.UpdateBibiteCap(0);
                }
                if (!ParseUtility.TryParse(a, out int l, out Exception e))
                {
                    Main.LogWarning("Could not parse value: " + a);
                    LimitInput.Text = BibiteSpawner.capNumber.ToString();
                    return;
                }

                Commands.UpdateBibiteCap(l);

            }, CapLimRow);
            
            // GameObject SelectColumn = UIFactory.CreateVerticalGroup(this.ContentRoot, "SelectColumn", false, false, true, true, 5, new Vector4(8, 8, 10, 5),
            //     default, TextAnchor.MiddleLeft);
            // UIFactory.SetLayoutGroup<VerticalLayoutGroup>(ContentRoot, true, false);
            Text selectLabel = UIFactory.CreateLabel(ContentRoot, "SelectLabel", "Select", TextAnchor.MiddleCenter);
            UIFactory.SetLayoutElement(selectLabel.gameObject, minHeight: 30, flexibleWidth: 9999);
            GameObject SelectRow = UIFactory.CreateHorizontalGroup(this.ContentRoot, "LayRow", false, false, true, true, 5, new Vector4(8, 8, 10, 5),
                default, TextAnchor.MiddleLeft);
            // selectLabel = UIFactory.CreateLabel(ContentRoot, "SelectLabel", "Select2", TextAnchor.MiddleCenter);
            ButtonRef? SelectClosestButton = UIFactory.CreateButton(SelectRow, "SelectClosestButton", "Closest");
            UIFactory.SetLayoutElement(SelectClosestButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            SelectClosestButton.Component.onClick.AddListener(() =>
            {
                GameObject? t = Tools.GetClosestEntity();
                if (t != null)
                    UserControl.Instance.SelectClosestEntity();
            });
            
            ButtonRef? SelectRandomButton = UIFactory.CreateButton(SelectRow, "SelectRandomButton", "Random");
            UIFactory.SetLayoutElement(SelectRandomButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            SelectRandomButton.Component.onClick.AddListener(() =>
            {
                Commands.SelectRandomEntity();
            });
            
            ButtonRef? SelectNoneButton = UIFactory.CreateButton(SelectRow, "SelectNoneButton", "None");
            UIFactory.SetLayoutElement(SelectNoneButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            SelectNoneButton.Component.onClick.AddListener(() =>
            {
                Main.isCinematic = false;
                UserControl.Instance.ClearTarget();
            });
            
            ButtonRef? SelectOldestButton = UIFactory.CreateButton(SelectRow, "SelectOldestButton", "Oldest");
            UIFactory.SetLayoutElement(SelectOldestButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            SelectOldestButton.Component.onClick.AddListener(() =>
            {
                GameObject? a = Tools.GetOldestBitbite();
                if (a != null)
                    UserControl.Instance.SelectTarget(a);
            });
            
            ButtonRef? SelectHighestButton = UIFactory.CreateButton(SelectRow, "SelectHighestButton", "Highest Gen");
            UIFactory.SetLayoutElement(SelectHighestButton.Component.gameObject, minHeight: 25, minWidth: 90, flexibleWidth: 9999);
            SelectHighestButton.Component.onClick.AddListener(() =>
            {
                GameObject? a = Tools.GetHighestGenerationBitbite();
                if (a != null)
                    UserControl.Instance.SelectTarget(a);
            });
            
            
            




        }

        public override void OnFinishResize()
        {
            // var rect = Rect.rect;
            // Main.Log("Resized Tools Panel to " + rect.width + "x" + rect.height);
            base.OnFinishResize();
        }


        public override void Update()
        {
            CinematicToggle.isOn = Main.isCinematic;
            // CinematicInterval.Text = Main.cinematicInterval.ToString();
            
            //update the text of the input field if the value is different
            if (!CinematicInterval.Component.isFocused)
            {
                if (Main.cinematicInterval == 0)
                {
                    CinematicInterval.Text = "Disabled";
                } else
                {
                    CinematicInterval.Text = Main.cinematicInterval.ToString() + "s";
                }
            }
            
            if (!LimitInput.Component.isFocused)
            {
                if (BibiteSpawner.limitNumber == 0)
                {
                    LimitInput.Text = "Disabled";
                } else
                {
                    LimitInput.Text = BibiteSpawner.limitNumber.ToString();
                }
            }
            
            if (!CapInput.Component.isFocused)
            {
                if (BibiteSpawner.capNumber == 0)
                {
                    CapInput.Text = "Disabled";
                } else
                {
                    CapInput.Text = BibiteSpawner.capNumber.ToString();
                }
            }
            
            
        }
        
        


        void AddSpacer(int height)
        {
            GameObject obj = UIFactory.CreateUIObject("Spacer", ContentRoot);
            UIFactory.SetLayoutElement(obj, minHeight: height, flexibleHeight: 0);
        }
        
        
        GameObject AddInputField(string name, string labelText, string placeHolder, out InputFieldRef inputField, Action<string> onInputEndEdit, GameObject root)
        {
            GameObject row = UIFactory.CreateHorizontalGroup(root, $"{name}_Group", false, false, true, true, 3, default, new(1, 1, 1, 0));

            Text posLabel = UIFactory.CreateLabel(row, $"{name}_Label", labelText);
            UIFactory.SetLayoutElement(posLabel.gameObject, minWidth: 100, minHeight: 25);

            inputField = UIFactory.CreateInputField(row, $"{name}_Input", placeHolder);
            UIFactory.SetLayoutElement(inputField.GameObject, minWidth: 125, minHeight: 25, flexibleWidth: 9999);
            inputField.Component.GetOnEndEdit().AddListener(onInputEndEdit);

            return row;
        }
        
        GameObject AddInputField(string name, string labelText, string placeHolder, out InputFieldRef inputField, Action<string> onInputEndEdit)
        {
            return AddInputField(name, labelText, placeHolder, out inputField, onInputEndEdit, ContentRoot);
        }
    }
}