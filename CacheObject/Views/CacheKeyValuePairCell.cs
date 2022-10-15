using TwitchIntegration.CacheObject.IValues;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI.Models;

namespace TwitchIntegration.CacheObject.Views
{
    public class CacheKeyValuePairCell : CacheObjectCell
    {
        public Image Image { get; private set; }
        public InteractiveDictionary DictOwner => Occupant.Owner as InteractiveDictionary;

        public LayoutElement KeyGroupLayout;
        public Text KeyLabel;
        public ButtonRef KeyInspectButton;
        public InputFieldRef KeyInputField;
        public Text KeyInputTypeLabel;

        public static Color EvenColor = new(0.07f, 0.07f, 0.07f);
        public static Color OddColor = new(0.063f, 0.063f, 0.063f);

        public int AdjustedWidth => (int)Rect.rect.width - 70;
        
        public override GameObject CreateContent(GameObject parent)
        {
            Main.LogError("This shouldn't have ran - CacheKeyValuePairCell.CreateContent");
            return parent;
        }

        protected override void ConstructEvaluateHolder(GameObject parent)
        {
            // not used
        }
    }
}
