/* ----- ----- ----- ----- */
// HelpMenuPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/14
// Update Date: 2026/10/14
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Menus;
using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>Selects the help tab; content entries have no action yet.</summary>
    public sealed class HelpMenuPresenter
    {
        public HelpMenuTab Tab { get; private set; } = HelpMenuTab.Lessons;
        public event Action Changed;

        public HelpMenuModel Model
        {
            get
            {
                var entries = new List<MenuEntry<HelpMenuId>>();
                foreach (var item in Tab == HelpMenuTab.Lessons ? HelpCatalog.Lessons : HelpCatalog.ModeIntroductions)
                    entries.Add(new(item.Id, item.Label, () => { }));
                return new(Tab, Array.AsReadOnly(new[] { Lang.Get("help.lessons"), Lang.Get("help.modes") }), entries.AsReadOnly());
            }
        }

        public void Open()
        {
            Tab = HelpMenuTab.Lessons;
            Changed?.Invoke();
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index > 1 || (int)Tab == index) return;
            Tab = (HelpMenuTab)index;
            Changed?.Invoke();
        }
    }
}
