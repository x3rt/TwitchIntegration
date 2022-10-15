using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;

namespace TwitchIntegration.CacheObject.Views
{
    public abstract class CacheObjectCell : ICell
    {
        #region ICell

        public float DefaultHeight => 30f;

        public GameObject UIRoot { get; set; }

        public bool Enabled => m_enabled;
        private bool m_enabled;

        public RectTransform Rect { get; set; }

        public void Disable()
        {
            m_enabled = false;
            UIRoot.SetActive(false);
        }

        public void Enable()
        {
            m_enabled = true;
            UIRoot.SetActive(true);
        }

        #endregion

        public CacheObjectBase Occupant { get; set; }
        public bool SubContentActive => SubContentHolder.activeSelf;

        public LayoutElement NameLayout;
        public GameObject RightGroupContent;
        public LayoutElement RightGroupLayout;
        public GameObject SubContentHolder;

        public Text NameLabel;
        public InputFieldRef HiddenNameLabel; // for selecting the name label
        public Text TypeLabel;
        public Text ValueLabel;
        public Toggle Toggle;
        public Text ToggleText;
        public InputFieldRef InputField;

        public ButtonRef InspectButton;
        public ButtonRef SubContentButton;
        public ButtonRef ApplyButton;

        public ButtonRef CopyButton;
        public ButtonRef PasteButton;

        public readonly Color subInactiveColor = new(0.23f, 0.23f, 0.23f);
        public readonly Color subActiveColor = new(0.23f, 0.33f, 0.23f);

        protected virtual void ApplyClicked()
        {
            Occupant.OnCellApplyClicked();
        }

        protected virtual void ToggleClicked(bool value)
        {
            ToggleText.text = value.ToString();
        }

        protected virtual void SubContentClicked()
        {
            this.Occupant.OnCellSubContentToggle();
        }

        public void RefreshSubcontentButton()
        {
            this.SubContentButton.ButtonText.text = SubContentHolder.activeSelf ? "▼" : "▲";
            Color color = SubContentHolder.activeSelf ? subActiveColor : subInactiveColor;
            RuntimeHelper.SetColorBlock(SubContentButton.Component, color, color * 1.3f);
        }

        protected abstract void ConstructEvaluateHolder(GameObject parent);

        public virtual GameObject CreateContent(GameObject parent)
        {
            Main.LogError("This shouldn't have ran... - CacheObjectCell.CreateContent");
            return UIRoot;
        }
    }
}
