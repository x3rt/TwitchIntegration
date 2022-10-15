using UnityEngine;
using UniverseLib.UI.Panels;

namespace TwitchIntegration.UI.Panels
{
    public class TIPanelDragger : PanelDragger
    {
        public TIPanelDragger (PanelBase uiPanel) : base(uiPanel) { }
        
        protected override bool MouseInResizeArea(Vector2 mousePos)
        {
            return !UIManager.NavBarRect.rect.Contains(UIManager.NavBarRect.InverseTransformPoint(mousePos))
                   && base.MouseInResizeArea(mousePos);
        }
        
    }
}