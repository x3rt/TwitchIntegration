using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace TwitchIntegration.UI.Panels
{
    public class StatsPanel : TIPanel
    {
        public StatsPanel (UIBase owner) : base(owner)
        {
        }

        public override UIManager.Panels PanelType => UIManager.Panels.Stats;
        
        private Text myText;
        int fontSize = 14;

        protected override void ConstructPanelContent()
        {
            //GameObject firstRow = UIFactory.CreateHorizontalGroup(ContentRoot, "FirstRow", false, false, true, true, 5, new(2, 2, 2, 2), new(1, 1, 1, 0));
            // GameObject topPart = UIFactory.CreateHorizontalGroup(ContentRoot, "TopPart", false, false, true, false, 5, new(2, 2, 2, 2), new(1, 1, 1, 0));
            // UIFactory.SetLayoutElement(topPart, minHeight: 25, flexibleWidth: 999);
            
            GameObject firstRow = UIFactory.CreateHorizontalGroup(ContentRoot, "FirstRow", false, false, true, true, 5, new(2, 2, 2, 2), new(1, 1, 1, 0), childAlignment: TextAnchor.UpperLeft);
            UIFactory.SetLayoutElement(firstRow, minHeight: 25, flexibleWidth: 0, flexibleHeight:0);
            
            //Content Size Fitter
            ContentSizeFitter contentSizeFitter = this.Rect.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            myText = UIFactory.CreateLabel(firstRow, "statsText", "Start a game to see stats");
            UIFactory.SetLayoutElement(myText.gameObject, minWidth: 200, minHeight: 25);
            myText.fontSize = fontSize;

            GameObject bottomPart = UIFactory.CreateHorizontalGroup(ContentRoot, "BottomPart", false, false, false, false, 0, new(2, 2, 2, 2), childAlignment: TextAnchor.LowerRight);
            
            ButtonRef minusButton = UIFactory.CreateButton(bottomPart, "MinusIncrementButton", "-");
            UIFactory.SetLayoutElement(minusButton.GameObject, minWidth: 20, flexibleWidth: 0, minHeight: 25);
            minusButton.OnClick += () => fontSize--;

            ButtonRef plusButton = UIFactory.CreateButton(bottomPart, "PlusIncrementButton", "+");
            UIFactory.SetLayoutElement(plusButton.GameObject, minWidth: 20, flexibleWidth: 0, minHeight: 25);
            plusButton.OnClick += () => fontSize++;
            
            
        }

        public override string Name => "Stats";

        public override int MinWidth => 100;
        public override int MinHeight => 100;
        public override Vector2 DefaultAnchorMin => new(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new(0.5f, 0.5f);
        
        public override bool CanDragAndResize => true;
        public override bool NavButtonWanted => true;
        public override bool ShouldSaveActiveState => true;
        public override bool ShowByDefault => true;
        public override void Update()
        {
            string s = "";
            
            if (Statistics.Instance != null)
            {
                s += "All Time highest generation: " + Statistics.Instance.AllTimeHighestGeneration + "\n";
                s += "Session highest generation: " + Statistics.Instance.SessionHighestGeneration + "\n";
                s += "Current highest generation: " + Statistics.Instance.CurrentHighestGeneration + "\n";
                s += "All Time highest population: " + Statistics.Instance.AllTimeHighestPopulation + "\n";
                s += "Session highest population: " + Statistics.Instance.SessionHighestPopulation + "\n";
                s += "All Time highest age: " + Statistics.Instance.AllTimeHighestAge + "\n";
                s += "Session highest age: " + Statistics.Instance.SessionHighestAge + "\n";
                s += "Current highest age: " + Statistics.Instance.CurrentHighestAge + "\n";
            }
            else
            {
                s = "Start a game to see stats";
            }

            myText.text = s;
            myText.fontSize = fontSize;
            base.Update();
        }
    }
}