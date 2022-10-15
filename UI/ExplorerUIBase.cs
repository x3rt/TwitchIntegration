using System;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace TwitchIntegration.UI
{
    internal class ExplorerUIBase : UIBase
    {
        public ExplorerUIBase(string id, Action updateMethod) : base(id, updateMethod) { }

        protected override PanelManager CreatePanelManager()
        {
            return new TIPanelManager(this);
        }
    }
}
