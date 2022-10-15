﻿using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace TwitchIntegration.UI
{
    public class TIPanelManager : PanelManager
    {
        public TIPanelManager(UIBase owner) : base(owner) { }

        protected override Vector3 MousePosition => DisplayManager.MousePosition;

        protected override Vector2 ScreenDimensions => new(DisplayManager.Width, DisplayManager.Height);

        protected override bool MouseInTargetDisplay => DisplayManager.MouseInTargetDisplay;

        internal void DoInvokeOnPanelsReordered()
        {
            InvokeOnPanelsReordered();
        }

        protected override void SortDraggerHeirarchy()
        {
            base.SortDraggerHeirarchy();

            // move AutoCompleter to first update
            // if (!UIManager.Initializing && AutoCompleteModal.Instance != null)
            // {
            //     this.draggerInstances.Remove(AutoCompleteModal.Instance.Dragger);
            //     this.draggerInstances.Insert(0, AutoCompleteModal.Instance.Dragger);
            // }
        }
    }
}